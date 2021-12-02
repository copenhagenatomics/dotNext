using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using static System.Threading.Timeout;

namespace DotNext.Threading;

using Tasks;
using Tasks.Pooling;
using Timestamp = Diagnostics.Timestamp;

/// <summary>
/// Provides a framework for implementing asynchronous locks and related synchronization primitives that rely on first-in-first-out (FIFO) wait queues.
/// </summary>
public class QueuedSynchronizer : Disposable
{
    private protected abstract class WaitNode : LinkedValueTaskCompletionSource<bool>
    {
        private Timestamp createdAt;
        private Action<double>? lockDurationCounter;
        private bool throwOnTimeout;

        // stores information about suspended caller for debugging purposes
        internal object? CallerInfo
        {
            get;
            private set;
        }

        private protected override void ResetCore()
        {
            lockDurationCounter = null;
            CallerInfo = null;
            base.ResetCore();
        }

        internal void Initialize(bool throwOnTimeout, Action<double>? lockDurationCounter, object? callerInfo)
        {
            this.throwOnTimeout = throwOnTimeout;
            this.lockDurationCounter = lockDurationCounter;
            CallerInfo = callerInfo;
            createdAt = Timestamp.Current;
        }

        protected sealed override Result<bool> OnTimeout() => throwOnTimeout ? base.OnTimeout() : false;

        private void ReportLockDuration()
            => lockDurationCounter?.Invoke(createdAt.Elapsed.TotalMilliseconds);

        private protected static void AfterConsumed<T>(T node)
            where T : WaitNode, IPooledManualResetCompletionSource<T>
        {
            node.ReportLockDuration();
            node.As<IPooledManualResetCompletionSource<T>>().OnConsumed?.Invoke(node);
        }
    }

    private protected sealed class DefaultWaitNode : WaitNode, IPooledManualResetCompletionSource<DefaultWaitNode>
    {
        private Action<DefaultWaitNode>? consumedCallback;

        protected sealed override void AfterConsumed() => AfterConsumed(this);

        ref Action<DefaultWaitNode>? IPooledManualResetCompletionSource<DefaultWaitNode>.OnConsumed => ref consumedCallback;
    }

    private protected interface ILockManager
    {
        bool IsLockAllowed { get; }

        void AcquireLock();
    }

    private protected interface ILockManager<in TNode> : ILockManager
        where TNode : WaitNode
    {
        void InitializeNode(TNode node);
    }

    private readonly Action<double>? contentionCounter, lockDurationCounter;
    private readonly TaskCompletionSource disposeTask;
    private readonly ThreadLocal<object?> callerInfo;
    private bool trackSuspendedCallers;
    private protected LinkedValueTaskCompletionSource<bool>? first;
    private LinkedValueTaskCompletionSource<bool>? last;

    private protected QueuedSynchronizer()
    {
        disposeTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
        callerInfo = new(false);
    }

    // aggressive inlining allows to devirt DrainWaitQueue call
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private protected void RemoveAndDrainWaitQueue(LinkedValueTaskCompletionSource<bool> node)
    {
        Debug.Assert(Monitor.IsEntered(this));

        if (RemoveNodeCore(node))
            DrainWaitQueue();
    }

    private protected bool IsDisposeRequested
    {
        get;
        private set;
    }

    /// <summary>
    /// Enables capturing information about suspended callers in DEBUG configuration.
    /// </summary>
    [Conditional("DEBUG")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void TrackSuspendedCallers() => trackSuspendedCallers = true;

    /// <summary>
    /// Sets caller information in DEBUG configuration.
    /// </summary>
    /// <remarks>
    /// It is recommended to inject caller information immediately before calling of <c>WaitAsync</c> method.
    /// </remarks>
    /// <param name="information">The object that identifies the caller.</param>
    [Conditional("DEBUG")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void SetCallerInformation(object information)
    {
        ArgumentNullException.ThrowIfNull(information);

        callerInfo.Value = information;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private IReadOnlyList<object?> GetSuspendedCallersCore()
    {
        if (first is null)
            return Array.Empty<Activity?>();

        var list = new List<object?>();
        for (LinkedValueTaskCompletionSource<bool>? current = first; current is not null; current = current.Next)
        {
            if (current is WaitNode node)
                list.Add(node.CallerInfo);
        }

        list.TrimExcess();
        return list;
    }

    /// <summary>
    /// Gets a list of suspended callers respecting their order in wait queue.
    /// </summary>
    /// <remarks>
    /// This method is introduced for debugging purposes only.
    /// </remarks>
    /// <returns>A list of suspended callers.</returns>
    /// <seealso cref="TrackSuspendedCallers"/>
    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IReadOnlyList<object?> GetSuspendedCallers()
        => trackSuspendedCallers ? GetSuspendedCallersCore() : Array.Empty<object?>();

    /// <summary>
    /// Sets counter for lock contention.
    /// </summary>
    public IncrementingEventCounter LockContentionCounter
    {
        init => contentionCounter = (value ?? throw new ArgumentNullException(nameof(value))).Increment;
    }

    /// <summary>
    /// Sets counter of lock duration, in milliseconds.
    /// </summary>
    public EventCounter LockDurationCounter
    {
        init => lockDurationCounter = (value ?? throw new ArgumentNullException(nameof(value))).WriteMetric;
    }

    private bool RemoveNodeCore(LinkedValueTaskCompletionSource<bool> node)
    {
        bool isFirst;

        if (isFirst = ReferenceEquals(first, node))
            first = node.Next;

        if (ReferenceEquals(last, node))
            last = node.Previous;

        node.Detach();
        return isFirst;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private protected bool RemoveNode(LinkedValueTaskCompletionSource<bool> node) => RemoveNodeCore(node);

    private protected virtual void DrainWaitQueue() => Debug.Assert(Monitor.IsEntered(this));

    private TNode EnqueueNode<TNode, TLockManager>(ref ValueTaskPool<bool, TNode> pool, ref TLockManager manager, bool throwOnTimeout, object? callerInfo)
        where TNode : WaitNode, IPooledManualResetCompletionSource<TNode>, new()
        where TLockManager : struct, ILockManager<TNode>
    {
        Debug.Assert(Monitor.IsEntered(this));

        var node = pool.Get();
        manager.InitializeNode(node);
        node.Initialize(throwOnTimeout, lockDurationCounter, callerInfo);

        if (last is null)
        {
            first = last = node;
        }
        else
        {
            last.Append(node);
            last = node;
        }

        contentionCounter?.Invoke(1L);
        return node;
    }

    private protected bool TryAcquire<TLockManager>(ref TLockManager manager)
        where TLockManager : struct, ILockManager
    {
        Debug.Assert(Monitor.IsEntered(this));

        bool result;

        if (result = manager.IsLockAllowed)
        {
            for (LinkedValueTaskCompletionSource<bool>? current = first, next; current is not null; current = next)
            {
                next = current.Next;

                if (current.IsCompleted)
                {
                    RemoveNodeCore(current);
                }
                else
                {
                    result = false;
                    goto exit;
                }
            }

            manager.AcquireLock();
        }

    exit:
        return result;
    }

    private object? CaptureCallerInfo()
    {
        var result = callerInfo.Value;
        if (result is null)
        {
            result = Activity.Current ?? Trace.CorrelationManager.LogicalOperationStack.Peek();
        }
        else
        {
            callerInfo.Value = null;
        }

        return result;
    }

    private protected ValueTask WaitWithTimeoutAsync<TNode, TLockManager>(ref TLockManager manager, ref ValueTaskPool<bool, TNode> pool, TimeSpan timeout, CancellationToken token)
        where TNode : WaitNode, IPooledManualResetCompletionSource<TNode>, new()
        where TLockManager : struct, ILockManager<TNode>
    {
        Debug.Assert(Monitor.IsEntered(this));

        var callerInfo = trackSuspendedCallers ? CaptureCallerInfo() : null;

        if (IsDisposed || IsDisposeRequested)
            return new(DisposedTask);

        if (timeout < TimeSpan.Zero && timeout != InfiniteTimeSpan)
            return ValueTask.FromException(new ArgumentOutOfRangeException(nameof(timeout)));

        if (token.IsCancellationRequested)
            return ValueTask.FromCanceled(token);

        if (TryAcquire(ref manager))
            return ValueTask.CompletedTask;

        if (timeout == TimeSpan.Zero)
            return ValueTask.FromException(new TimeoutException());

        return EnqueueNode(ref pool, ref manager, throwOnTimeout: true, callerInfo).CreateVoidTask(timeout, token);
    }

    private protected ValueTask<bool> WaitNoTimeoutAsync<TNode, TManager>(ref TManager manager, ref ValueTaskPool<bool, TNode> pool, TimeSpan timeout, CancellationToken token)
        where TNode : WaitNode, IPooledManualResetCompletionSource<TNode>, new()
        where TManager : struct, ILockManager<TNode>
    {
        Debug.Assert(Monitor.IsEntered(this));

        var callerInfo = trackSuspendedCallers ? CaptureCallerInfo() : null;

        if (IsDisposed || IsDisposeRequested)
            return new(GetDisposedTask<bool>());

        if (timeout < TimeSpan.Zero && timeout != InfiniteTimeSpan)
            return ValueTask.FromException<bool>(new ArgumentOutOfRangeException(nameof(timeout)));

        if (token.IsCancellationRequested)
            return ValueTask.FromCanceled<bool>(token);

        if (TryAcquire(ref manager))
            return new(true);

        if (timeout == TimeSpan.Zero)
            return new(false);    // if timeout is zero fail fast

        return EnqueueNode(ref pool, ref manager, throwOnTimeout: false, callerInfo).CreateTask(timeout, token);
    }

    /// <summary>
    /// Cancels all suspended callers.
    /// </summary>
    /// <param name="token">The canceled token.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="token"/> is not in canceled state.</exception>
    /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void CancelSuspendedCallers(CancellationToken token)
    {
        ThrowIfDisposed();

        if (!token.IsCancellationRequested)
            throw new ArgumentOutOfRangeException(nameof(token));

        unsafe
        {
            DrainWaitQueue(&TrySetCanceled, token);
        }

        static bool TrySetCanceled(LinkedValueTaskCompletionSource<bool> source, CancellationToken token)
            => source.TrySetCanceled(token);
    }

    private protected long ResumeSuspendedCallers()
    {
        unsafe
        {
            return DrainWaitQueue(&TrySetResult, true);
        }

        static bool TrySetResult(LinkedValueTaskCompletionSource<bool> source, bool result)
            => source.TrySetResult(result);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void NotifyObjectDisposed()
    {
        var e = new ObjectDisposedException(GetType().Name);

        unsafe
        {
            DrainWaitQueue(&TrySetException, e);
        }

        static bool TrySetException(LinkedValueTaskCompletionSource<bool> source, ObjectDisposedException e)
            => source.TrySetException(e);
    }

    private unsafe long DrainWaitQueue<T>(delegate*<LinkedValueTaskCompletionSource<bool>, T, bool> callback, T arg)
    {
        Debug.Assert(Monitor.IsEntered(this));
        Debug.Assert(callback != null);

        var count = 0L;

        for (LinkedValueTaskCompletionSource<bool>? current = first, next; current is not null; current = next)
        {
            next = current.Next;

            if (callback(current, arg))
                count++;
        }

        first = last = null;
        return count;
    }

    /// <summary>
    /// Releases all resources associated with exclusive lock.
    /// </summary>
    /// <remarks>
    /// This method is not thread-safe and may not be used concurrently with other members of this instance.
    /// </remarks>
    /// <param name="disposing">Indicates whether the <see cref="Dispose(bool)"/> has been called directly or from finalizer.</param>
    protected override void Dispose(bool disposing)
    {
        IsDisposeRequested = true;

        if (disposing)
        {
            NotifyObjectDisposed();
            disposeTask.TrySetResult();
            callerInfo?.Dispose();
        }

        base.Dispose(disposing);
    }

    private protected virtual bool IsReadyToDispose => true;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.Synchronized)]
    protected override ValueTask DisposeAsyncCore()
    {
        IsDisposeRequested = true;

        if (IsReadyToDispose)
        {
            Dispose(true);
            return ValueTask.CompletedTask;
        }

        return new(disposeTask.Task);
    }

    /// <summary>
    /// Disposes this synchronization primitive gracefully.
    /// </summary>
    /// <returns>The task representing asynchronous result.</returns>
    public new ValueTask DisposeAsync() => base.DisposeAsync();
}