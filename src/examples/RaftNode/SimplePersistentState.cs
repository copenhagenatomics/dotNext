using DotNext;
using DotNext.IO;
using DotNext.Net.Cluster.Consensus.Raft;
using System.Diagnostics;

namespace RaftNode;

internal sealed class SimplePersistentState : MemoryBasedStateMachine, ISupplier<byte[]>
{
    internal const string LogLocation = "logLocation";

    private sealed class SimpleSnapshotBuilder : IncrementalSnapshotBuilder
    {
        private byte[] value = Array.Empty<byte>();

        public SimpleSnapshotBuilder(in SnapshotBuilderContext context)
            : base(context)
        {
        }

        protected override async ValueTask ApplyAsync(LogEntry entry)
            => value = await entry.ToByteArrayAsync().ConfigureAwait(false);

        public override ValueTask WriteToAsync<TWriter>(TWriter writer, CancellationToken token)
            => writer.WriteAsync(value, null, token);
    }

    private int entriesApplied;
    private readonly object contentLock = new ();
    private byte[] content = Array.Empty<byte>();
    private readonly Stopwatch timeTo1kValues = new();

    public SimplePersistentState(string path, AppEventSource source)
        : base(path, 50, CreateOptions(source))
    {
    }

    public SimplePersistentState(IConfiguration configuration, AppEventSource source)
        : this(configuration[LogLocation], source)
    {
    }

    private static Options CreateOptions(AppEventSource source)
    {
        var result = new Options
        {   
            BufferSize = 32 * 1024,
            InitialPartitionSize = 50 * System.Runtime.CompilerServices.Unsafe.SizeOf<BigStruct>(),
            CompactionMode = CompactionMode.Sequential,//sequential is the default
            WriteMode = WriteMode.AutoFlush,
            MaxConcurrentReads = 3,
            CacheEvictionPolicy = LogEntryCacheEvictionPolicy.OnSnapshot,
            WriteCounter = new("WAL.Writes", source),
            ReadCounter = new("WAL.Reads", source),
            CommitCounter = new("WAL.Commits", source),
            CompactionCounter = new("WAL.Compaction", source),
            LockContentionCounter = new("WAL.LockContention", source),
            LockDurationCounter = new("WAL.LockDuration", source),
        };

        Console.WriteLine($"InitialPartitionSize = {result.InitialPartitionSize}");

        result.WriteCounter.DisplayUnits =
            result.ReadCounter.DisplayUnits =
            result.CommitCounter.DisplayUnits =
            result.CompactionCounter.DisplayUnits = "entries";

        result.LockDurationCounter.DisplayUnits = "milliseconds";
        result.LockDurationCounter.DisplayName = "WAL Lock Duration";

        result.LockContentionCounter.DisplayName = "Lock Contention";

        result.WriteCounter.DisplayName = "Number of written entries";
        result.ReadCounter.DisplayName = "Number of retrieved entries";
        result.CommitCounter.DisplayName = "Number of committed entries";
        result.CompactionCounter.DisplayName = "Number of squashed entries";

        return result;
    }

    byte[] ISupplier<byte[]>.Invoke() 
    { 
        lock (contentLock) return content; 
    }

    private async ValueTask UpdateValue(LogEntry entry)
    {
        var value = await entry.ToByteArrayAsync().ConfigureAwait(false);
        lock (contentLock)
            content = value;
        var newEntryNumber = Interlocked.Increment(ref entriesApplied);
        if (newEntryNumber % 1000 == 0)
        {
            Console.WriteLine($"Accepting entry number {newEntryNumber} - time since last 1k value: {timeTo1kValues.Elapsed}");
            timeTo1kValues.Restart();
        }
    }

    protected override ValueTask ApplyAsync(LogEntry entry)
        => entry.Length == 0L ? new ValueTask() : UpdateValue(entry);

    protected override SnapshotBuilder CreateSnapshotBuilder(in SnapshotBuilderContext context)
    {
        return new SimpleSnapshotBuilder(context);
    }
}