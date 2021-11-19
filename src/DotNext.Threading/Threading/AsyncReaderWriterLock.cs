﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Threading.Timeout;
using Debug = System.Diagnostics.Debug;

namespace DotNext.Threading;

using Tasks.Pooling;

/// <summary>
/// Represents asynchronous version of <see cref="ReaderWriterLockSlim"/>.
/// </summary>
/// <remarks>
/// This lock doesn't support recursion.
/// </remarks>
public class AsyncReaderWriterLock : QueuedSynchronizer, IAsyncDisposable
{
    private enum LockType : byte
    {
        Read = 0,
        Upgrade,
        Exclusive,
    }

    private new sealed class WaitNode : QueuedSynchronizer.WaitNode, IPooledManualResetCompletionSource<WaitNode>
    {
        private Action<WaitNode>? consumedCallback;
        internal LockType Type;

        protected override void AfterConsumed()
        {
            ReportLockDuration();
            consumedCallback?.Invoke(this);
            CallerInfo = null;
        }

        private protected override void ResetCore()
        {
            consumedCallback = null;
            base.ResetCore();
        }

        Action<WaitNode>? IPooledManualResetCompletionSource<WaitNode>.OnConsumed
        {
            set => consumedCallback = value;
        }

        internal bool IsReadLock => Type != LockType.Exclusive;

        internal bool IsUpgradeableLock => Type == LockType.Upgrade;

        internal bool IsWriteLock => Type == LockType.Exclusive;
    }

    // describes internal state of reader/writer lock
    internal sealed class State
    {
        private long version;  // version of write lock

        // number of acquired read locks
        private long readLocks; // volatile
        private volatile bool writeLock;

        internal State(long version)
        {
            this.version = version;
            writeLock = false;
            readLocks = 0L;
        }

        internal bool WriteLock => writeLock;

        internal void DowngradeFromWriteLock()
        {
            writeLock = false;
            ReadLocks = 1L;
        }

        internal void ExitLock()
        {
            if (writeLock)
            {
                writeLock = false;
                ReadLocks = 0L;
            }
            else
            {
                readLocks.DecrementAndGet();
            }
        }

        internal long ReadLocks
        {
            get => readLocks.VolatileRead();
            private set => readLocks.VolatileWrite(value);
        }

        internal long Version => version.VolatileRead();

        internal bool IsWriteLockAllowed => !writeLock && ReadLocks == 0L;

        internal bool IsUpgradeToWriteLockAllowed => !writeLock && ReadLocks == 1L;

        internal void AcquireWriteLock()
        {
            readLocks.VolatileWrite(0L);
            writeLock = true;
            version.IncrementAndGet();
        }

        internal bool IsReadLockAllowed => !writeLock;

        internal void AcquireReadLock() => readLocks.IncrementAndGet();
    }

    [StructLayout(LayoutKind.Auto)]
    private readonly struct ReadLockManager : ILockManager<WaitNode>
    {
        private readonly State state;

        internal ReadLockManager(State state) => this.state = state;

        bool ILockManager.IsLockAllowed => state.IsReadLockAllowed;

        void ILockManager.AcquireLock() => state.AcquireReadLock();

        void ILockManager<WaitNode>.InitializeNode(WaitNode node)
            => node.Type = LockType.Read;
    }

    [StructLayout(LayoutKind.Auto)]
    private readonly struct WriteLockManager : ILockManager<WaitNode>
    {
        private readonly State state;

        internal WriteLockManager(State state) => this.state = state;

        bool ILockManager.IsLockAllowed => state.IsWriteLockAllowed;

        void ILockManager.AcquireLock() => state.AcquireWriteLock();

        void ILockManager<WaitNode>.InitializeNode(WaitNode node)
            => node.Type = LockType.Exclusive;
    }

    [StructLayout(LayoutKind.Auto)]
    private readonly struct UpgradeManager : ILockManager<WaitNode>
    {
        private readonly State state;

        internal UpgradeManager(State state) => this.state = state;

        bool ILockManager.IsLockAllowed => state.IsUpgradeToWriteLockAllowed;

        void ILockManager.AcquireLock() => state.AcquireWriteLock();

        void ILockManager<WaitNode>.InitializeNode(WaitNode node)
            => node.Type = LockType.Upgrade;
    }

    /// <summary>
    /// Represents lock stamp used for optimistic reading.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct LockStamp : IEquatable<LockStamp>
    {
        private readonly long version;
        private readonly bool valid;

        internal LockStamp(in State state)
        {
            version = state.Version;
            valid = true;
        }

        internal bool IsValid(in State state)
            => valid && state.Version == version;

        private bool Equals(in LockStamp other) => version == other.version && valid == other.valid;

        /// <summary>
        /// Determines whether this stamp represents the same version of the lock state
        /// as the given stamp.
        /// </summary>
        /// <param name="other">The lock stamp to compare.</param>
        /// <returns><see langword="true"/> of this stamp is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        public bool Equals(LockStamp other) => Equals(in other);

        /// <summary>
        /// Determines whether this stamp represents the same version of the lock state
        /// as the given stamp.
        /// </summary>
        /// <param name="other">The lock stamp to compare.</param>
        /// <returns><see langword="true"/> of this stamp is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        public override bool Equals([NotNullWhen(true)] object? other) => other is LockStamp stamp && Equals(in stamp);

        /// <summary>
        /// Computes hash code for this stamp.
        /// </summary>
        /// <returns>The hash code of this stamp.</returns>
        public override int GetHashCode() => HashCode.Combine(valid, version);

        /// <summary>
        /// Determines whether the first stamp represents the same version of the lock state
        /// as the second stamp.
        /// </summary>
        /// <param name="first">The first lock stamp to compare.</param>
        /// <param name="second">The second lock stamp to compare.</param>
        /// <returns><see langword="true"/> of <paramref name="first"/> stamp is equal to <paramref name="second"/>; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(in LockStamp first, in LockStamp second)
            => first.Equals(in second);

        /// <summary>
        /// Determines whether the first stamp represents the different version of the lock state
        /// as the second stamp.
        /// </summary>
        /// <param name="first">The first lock stamp to compare.</param>
        /// <param name="second">The second lock stamp to compare.</param>
        /// <returns><see langword="true"/> of <paramref name="first"/> stamp is not equal to <paramref name="second"/>; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(in LockStamp first, in LockStamp second)
            => !first.Equals(in second);
    }

    private readonly ValueTaskPool<WaitNode> pool;
    private readonly State state;

    /// <summary>
    /// Initializes a new reader/writer lock.
    /// </summary>
    /// <param name="concurrencyLevel">The expected number of concurrent flows.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="concurrencyLevel"/> is less than or equal to zero.</exception>
    public AsyncReaderWriterLock(int concurrencyLevel)
    {
        if (concurrencyLevel <= 0)
            throw new ArgumentOutOfRangeException(nameof(concurrencyLevel));

        state = new(long.MinValue);
        pool = new ValueTaskPool<WaitNode>(concurrencyLevel, RemoveAndDrainWaitQueue);
    }

    /// <summary>
    /// Initializes a new reader/writer lock.
    /// </summary>
    public AsyncReaderWriterLock()
    {
        state = new(long.MinValue);
        pool = new(RemoveAndDrainWaitQueue);
    }

    /// <summary>
    /// Gets the total number of unique readers.
    /// </summary>
    public long CurrentReadCount => state.ReadLocks;

    /// <summary>
    /// Gets a value that indicates whether the read lock taken.
    /// </summary>
    public bool IsReadLockHeld => CurrentReadCount != 0L;

    /// <summary>
    /// Gets a value that indicates whether the write lock taken.
    /// </summary>
    public bool IsWriteLockHeld => state.WriteLock;

    /// <summary>
    /// Returns a stamp that can be validated later.
    /// </summary>
    /// <returns>Optimistic read stamp. May be invalid.</returns>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public LockStamp TryOptimisticRead()
    {
        ThrowIfDisposed();
        return state.WriteLock ? new LockStamp() : new LockStamp(in state);
    }

    /// <summary>
    /// Returns <see langword="true"/> if the lock has not been exclusively acquired since issuance of the given stamp.
    /// </summary>
    /// <param name="stamp">A stamp to check.</param>
    /// <returns><see langword="true"/> if the lock has not been exclusively acquired since issuance of the given stamp; else <see langword="false"/>.</returns>
    public bool Validate(in LockStamp stamp) => stamp.IsValid(in state);

    /// <summary>
    /// Attempts to obtain reader lock synchronously without blocking caller thread.
    /// </summary>
    /// <returns><see langword="true"/> if lock is taken successfuly; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool TryEnterReadLock()
    {
        ThrowIfDisposed();

        var manager = new ReadLockManager(state);
        return TryAcquire(ref manager);
    }

    /// <summary>
    /// Tries to enter the lock in read mode asynchronously, with an optional time-out.
    /// </summary>
    /// <param name="timeout">The interval to wait for the lock.</param>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns><see langword="true"/> if the caller entered read mode; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ValueTask<bool> TryEnterReadLockAsync(TimeSpan timeout, CancellationToken token = default)
    {
        var manager = new ReadLockManager(state);
        return WaitNoTimeoutAsync(ref manager, pool, timeout, token);
    }

    /// <summary>
    /// Enters the lock in read mode asynchronously.
    /// </summary>
    /// <param name="timeout">The interval to wait for the lock.</param>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns>The task representing asynchronous result.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="TimeoutException">The lock cannot be acquired during the specified amount of time.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ValueTask EnterReadLockAsync(TimeSpan timeout, CancellationToken token = default)
    {
        var manager = new ReadLockManager(state);
        return WaitWithTimeoutAsync(ref manager, pool, timeout, token);
    }

    /// <summary>
    /// Enters the lock in read mode asynchronously.
    /// </summary>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns>The task representing acquisition operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    public ValueTask EnterReadLockAsync(CancellationToken token = default)
        => EnterReadLockAsync(InfiniteTimeSpan, token);

    /// <summary>
    /// Attempts to acquire write lock without blocking.
    /// </summary>
    /// <param name="stamp">The stamp of the read lock.</param>
    /// <returns><see langword="true"/> if lock is acquired successfully; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool TryEnterWriteLock(in LockStamp stamp)
    {
        ThrowIfDisposed();
        if (stamp.IsValid(in state))
        {
            var manager = new WriteLockManager(state);
            return TryAcquire(ref manager);
        }

        return false;
    }

    /// <summary>
    /// Attempts to obtain writer lock synchronously without blocking caller thread.
    /// </summary>
    /// <returns><see langword="true"/> if lock is taken successfuly; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool TryEnterWriteLock()
    {
        ThrowIfDisposed();

        var manager = new WriteLockManager(state);
        return TryAcquire(ref manager);
    }

    /// <summary>
    /// Tries to enter the lock in write mode asynchronously, with an optional time-out.
    /// </summary>
    /// <param name="timeout">The interval to wait for the lock.</param>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns><see langword="true"/> if the caller entered write mode; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ValueTask<bool> TryEnterWriteLockAsync(TimeSpan timeout, CancellationToken token = default)
    {
        var manager = new WriteLockManager(state);
        return WaitNoTimeoutAsync(ref manager, pool, timeout, token);
    }

    /// <summary>
    /// Enters the lock in write mode asynchronously.
    /// </summary>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns>The task representing lock acquisition operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    public ValueTask EnterWriteLockAsync(CancellationToken token = default)
        => EnterWriteLockAsync(InfiniteTimeSpan, token);

    /// <summary>
    /// Enters the lock in write mode asynchronously.
    /// </summary>
    /// <param name="timeout">The interval to wait for the lock.</param>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns>The task representing lock acquisition operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="TimeoutException">The lock cannot be acquired during the specified amount of time.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ValueTask EnterWriteLockAsync(TimeSpan timeout, CancellationToken token = default)
    {
        var manager = new WriteLockManager(state);
        return WaitWithTimeoutAsync(ref manager, pool, timeout, token);
    }

    /// <summary>
    /// Tries to upgrade the read lock to the write lock synchronously without blocking of the caller.
    /// </summary>
    /// <returns><see langword="true"/> if lock is taken successfuly; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool TryUpgradeToWriteLock()
    {
        ThrowIfDisposed();

        var manager = new UpgradeManager(state);
        return TryAcquire(ref manager);
    }

    /// <summary>
    /// Tries to upgrade the read lock to the write lock asynchronously.
    /// </summary>
    /// <param name="timeout">The interval to wait for the lock.</param>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns><see langword="true"/> if the caller entered upgradeable mode; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ValueTask<bool> TryUpgradeToWriteLockAsync(TimeSpan timeout, CancellationToken token = default)
    {
        var manager = new UpgradeManager(state);
        return WaitNoTimeoutAsync(ref manager, pool, timeout, token);
    }

    /// <summary>
    /// Upgrades the read lock to the write lock asynchronously.
    /// </summary>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns>The task representing lock acquisition operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    public ValueTask UpgradeToWriteLockAsync(CancellationToken token = default)
        => UpgradeToWriteLockAsync(InfiniteTimeSpan, token);

    /// <summary>
    /// Upgrades the read lock to the write lock asynchronously.
    /// </summary>
    /// <param name="timeout">The interval to wait for the lock.</param>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns>The task representing lock acquisition operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="TimeoutException">The lock cannot be acquired during the specified amount of time.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ValueTask UpgradeToWriteLockAsync(TimeSpan timeout, CancellationToken token = default)
    {
        var manager = new UpgradeManager(state);
        return WaitWithTimeoutAsync(ref manager, pool, timeout, token);
    }

    private protected sealed override void DrainWaitQueue()
    {
        Debug.Assert(Monitor.IsEntered(this));

        for (WaitNode? current = first as WaitNode, next; current is not null; current = next)
        {
            next = current.Next as WaitNode;

            if (current.IsCompleted)
            {
                RemoveNode(current);
                continue;
            }

            switch (current.Type)
            {
                case LockType.Upgrade:
                    if (!state.IsUpgradeToWriteLockAllowed)
                        return;

                    if (current.TrySetResult(true))
                    {
                        RemoveNode(current);
                        state.AcquireWriteLock();
                        return;
                    }

                    continue;
                case LockType.Exclusive:
                    if (!state.IsWriteLockAllowed)
                        return;

                    // skip dead node
                    if (current.TrySetResult(true))
                    {
                        RemoveNode(current);
                        state.AcquireWriteLock();
                        return;
                    }

                    break;
                default:
                    if (!state.IsReadLockAllowed)
                        return;

                    if (current.TrySetResult(true))
                    {
                        RemoveNode(current);
                        state.AcquireReadLock();
                    }

                    continue;
            }
        }
    }

    /// <summary>
    /// Exits previously acquired mode.
    /// </summary>
    /// <remarks>
    /// Exiting from the lock is synchronous non-blocking operation.
    /// Lock acquisition is an asynchronous operation.
    /// </remarks>
    /// <exception cref="SynchronizationLockException">The caller has not entered the lock in write mode.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Release()
    {
        ThrowIfDisposed();

        if (state.IsWriteLockAllowed)
            throw new SynchronizationLockException(ExceptionMessages.NotInLock);

        state.ExitLock();
        DrainWaitQueue();

        if (IsDisposeRequested && IsReadyToDispose)
            Dispose(true);
    }

    /// <summary>
    /// Downgrades the write lock to the read lock.
    /// </summary>
    /// <remarks>
    /// Exiting from the lock is synchronous non-blocking operation.
    /// Lock acquisition is an asynchronous operation.
    /// </remarks>
    /// <exception cref="SynchronizationLockException">The caller has not entered the lock in write mode.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DowngradeFromWriteLock()
    {
        ThrowIfDisposed();

        if (state.IsWriteLockAllowed)
            throw new SynchronizationLockException(ExceptionMessages.NotInLock);

        state.DowngradeFromWriteLock();
        DrainWaitQueue();
    }

    private protected override bool IsReadyToDispose => state.IsWriteLockAllowed && first is null;
}