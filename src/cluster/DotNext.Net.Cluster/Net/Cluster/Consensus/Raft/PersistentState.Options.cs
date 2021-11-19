﻿using System.Buffers;
using System.Diagnostics.Tracing;
using System.IO.Compression;

namespace DotNext.Net.Cluster.Consensus.Raft;

using Buffers;
using IO.Log;

public partial class PersistentState
{
    internal interface IBufferManagerSettings
    {
        MemoryAllocator<T> GetMemoryAllocator<T>();

        bool UseCaching { get; }
    }

    internal interface IAsyncLockSettings
    {
        int ConcurrencyLevel { get; }

        IncrementingEventCounter? LockContentionCounter { get; }

        EventCounter? LockDurationCounter { get; }
    }

    /// <summary>
    /// Represents configuration options of the persistent audit trail.
    /// </summary>
    public class Options : IBufferManagerSettings, IAsyncLockSettings
    {
        private protected const int MinBufferSize = 128;
        private int bufferSize = 4096;
        private int concurrencyLevel = Math.Max(3, Environment.ProcessorCount);
        private long partitionSize;

        /// <summary>
        /// Gets or sets a value indicating usage of intermediate buffers during I/O.
        /// </summary>
        /// <value>
        /// <see langword="true"/> to bypass intermediate buffers for disk writes.
        /// </value>
        public bool WriteThrough
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets size of in-memory buffer for I/O operations.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is too small.</exception>
        public int BufferSize
        {
            get => bufferSize;
            set
            {
                if (value < MinBufferSize)
                    throw new ArgumentOutOfRangeException(nameof(value));
                bufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the initial size of the file that holds the partition with log entries, in bytes.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than zero.</exception>
        public long InitialPartitionSize
        {
            get => partitionSize;
            set => partitionSize = value >= 0L ? value : throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        /// Enables or disables in-memory cache.
        /// </summary>
        /// <value><see langword="true"/> to in-memory cache for faster read/write of log entries; <see langword="false"/> to reduce the memory by the cost of the performance.</value>
        public bool UseCaching { get; set; } = true;

        /// <summary>
        /// Gets memory allocator for internal purposes.
        /// </summary>
        /// <typeparam name="T">The type of items in the pool.</typeparam>
        /// <returns>The memory allocator.</returns>
        public virtual MemoryAllocator<T> GetMemoryAllocator<T>() => ArrayPool<T>.Shared.ToAllocator();

        /// <summary>
        /// Gets or sets the number of possible parallel reads.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 2.</exception>
        public int MaxConcurrentReads
        {
            get => concurrencyLevel;
            set
            {
                if (concurrencyLevel < 2)
                    throw new ArgumentOutOfRangeException(nameof(value));
                concurrencyLevel = value;
            }
        }

        /// <inheritdoc />
        int IAsyncLockSettings.ConcurrencyLevel => MaxConcurrentReads;

        /// <summary>
        /// Gets or sets compression level used
        /// to create backup archive.
        /// </summary>
        public CompressionLevel BackupCompression { get; set; } = CompressionLevel.Optimal;

        /// <summary>
        /// If set then every read operations will be performed
        /// on buffered copy of the log entries.
        /// </summary>
        public RaftLogEntriesBufferingOptions? CopyOnReadOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets lock contention counter.
        /// </summary>
        public IncrementingEventCounter? LockContentionCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets lock duration counter.
        /// </summary>
        public EventCounter? LockDurationCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the counter used to measure the number of retrieved log entries.
        /// </summary>
        public IncrementingEventCounter? ReadCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the counter used to measure the number of written log entries.
        /// </summary>
        public IncrementingEventCounter? WriteCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the counter used to measure the number of committed log entries.
        /// </summary>
        public IncrementingEventCounter? CommitCounter
        {
            get;
            set;
        }

        internal ILogEntryConsumer<IRaftLogEntry, (BufferedRaftLogEntryList, long?)>? CreateBufferingConsumer()
            => CopyOnReadOptions is null ? null : new BufferingLogEntryConsumer(CopyOnReadOptions);
    }
}