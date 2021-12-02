﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Threading.Timeout;

namespace DotNext.Threading;

using Tasks.Pooling;
using LinkedValueTaskCompletionSource = Tasks.LinkedValueTaskCompletionSource<bool>;

/// <summary>
/// Represents a synchronization primitive that is signaled when its count becomes non zero.
/// </summary>
/// <remarks>
/// This class behaves in opposite to <see cref="AsyncCountdownEvent"/>.
/// Every call of <see cref="Increment"/> increments the counter.
/// Every call of <see cref="WaitAsync(TimeSpan, CancellationToken)"/>
/// decrements counter and release the caller if the current count is greater than zero.
/// </remarks>
public class AsyncCounter : QueuedSynchronizer, IAsyncEvent
{
    [StructLayout(LayoutKind.Auto)]
    private struct StateManager : ILockManager<DefaultWaitNode>
    {
        private long state;

        internal StateManager(long initialValue)
            => state = initialValue;

        internal long Value
        {
            readonly get => state.VolatileRead();
            set => state.VolatileWrite(value);
        }

        internal void Increment() => state.IncrementAndGet();

        internal void Decrement() => state.DecrementAndGet();

        internal bool TryReset() => Interlocked.Exchange(ref state, 0L) > 0L;

        bool ILockManager.IsLockAllowed => Value > 0L;

        void ILockManager.AcquireLock() => Decrement();

        void ILockManager<DefaultWaitNode>.InitializeNode(DefaultWaitNode node)
        {
            // nothing to do here
        }
    }

    private ValueTaskPool<bool, DefaultWaitNode> pool;
    private StateManager manager;

    /// <summary>
    /// Initializes a new asynchronous counter.
    /// </summary>
    /// <param name="initialValue">The initial value of the counter.</param>
    /// <param name="concurrencyLevel">The expected number of concurrent flows.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="concurrencyLevel"/> is less than or equal to zero.</exception>
    public AsyncCounter(long initialValue, int concurrencyLevel)
    {
        if (initialValue < 0L)
            throw new ArgumentOutOfRangeException(nameof(initialValue));

        if (concurrencyLevel < 1)
            throw new ArgumentOutOfRangeException(nameof(concurrencyLevel));

        manager = new(initialValue);
        pool = new(OnCompleted, concurrencyLevel);
    }

    /// <summary>
    /// Initializes a new asynchronous counter.
    /// </summary>
    /// <param name="initialValue">The initial value of the counter.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialValue"/> is less than zero.</exception>
    public AsyncCounter(long initialValue = 0L)
    {
        if (initialValue < 0L)
            throw new ArgumentOutOfRangeException(nameof(initialValue));

        manager = new(initialValue);
        pool = new(OnCompleted);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void OnCompleted(DefaultWaitNode node)
    {
        RemoveAndDrainWaitQueue(node);
        pool.Return(node);
    }

    /// <inheritdoc/>
    bool IAsyncEvent.IsSet => manager.Value > 0L;

    /// <summary>
    /// Gets the counter value.
    /// </summary>
    /// <remarks>
    /// The returned value indicates how many calls you can perform
    /// using <see cref="WaitAsync(TimeSpan, CancellationToken)"/> without
    /// blocking.
    /// </remarks>
    public long Value => manager.Value;

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.Synchronized)]
    bool IAsyncEvent.Reset() => manager.TryReset();

    /// <summary>
    /// Increments counter and resume suspended callers.
    /// </summary>
    /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Increment()
    {
        ThrowIfDisposed();
        manager.Increment();

        for (LinkedValueTaskCompletionSource? current = first, next; current is not null && manager.Value > 0L; current = next)
        {
            next = current.Next;

            if (current.TrySetResult(true))
            {
                RemoveNode(current);
                manager.Decrement();
            }
        }
    }

    /// <inheritdoc/>
    bool IAsyncEvent.Signal()
    {
        Increment();
        return true;
    }

    /// <summary>
    /// Suspends caller if <see cref="Value"/> is zero
    /// or just decrements it.
    /// </summary>
    /// <param name="timeout">Time to wait for increment.</param>
    /// <param name="token">The token that can be used to cancel the waiting operation.</param>
    /// <returns><see langword="true"/> if counter is decremented successfully; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ValueTask<bool> WaitAsync(TimeSpan timeout, CancellationToken token = default)
        => WaitNoTimeoutAsync(ref manager, ref pool, timeout, token);

    /// <summary>
    /// Suspends caller if <see cref="Value"/> is zero
    /// or just decrements it.
    /// </summary>
    /// <param name="token">The token that can be used to cancel the waiting operation.</param>
    /// <returns>The task representing asynchronous result.</returns>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ValueTask WaitAsync(CancellationToken token = default)
        => WaitWithTimeoutAsync(ref manager, ref pool, InfiniteTimeSpan, token);

    /// <summary>
    /// Attempts to decrement the counter synchronously.
    /// </summary>
    /// <returns><see langword="true"/> if the counter decremented successfully; <see langword="false"/> if this counter is already zero.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool TryDecrement()
    {
        ThrowIfDisposed();
        return TryAcquire(ref manager);
    }
}