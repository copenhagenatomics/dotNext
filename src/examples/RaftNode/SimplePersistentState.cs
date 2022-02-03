using DotNext;
using DotNext.IO;
using DotNext.Net.Cluster.Consensus.Raft;

namespace RaftNode;

internal sealed class SimplePersistentState : MemoryBasedStateMachine, ISupplier<BigStruct>
{
    internal const string LogLocation = "logLocation";

    private sealed class SimpleSnapshotBuilder : IncrementalSnapshotBuilder
    {
        private BigStruct value;

        public SimpleSnapshotBuilder(in SnapshotBuilderContext context)
            : base(context)
        {
        }

        protected override async ValueTask ApplyAsync(LogEntry entry)
            => value = await entry.ToTypeAsync<BigStruct, LogEntry>().ConfigureAwait(false);

        public override ValueTask WriteToAsync<TWriter>(TWriter writer, CancellationToken token)
            => writer.WriteAsync(value, token);
    }

    private BigStruct content;

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
            InitialPartitionSize = 50 * System.Runtime.CompilerServices.Unsafe.SizeOf<BigStruct>(),
            BufferSize = 32 * 1024,
            WriteMode = WriteMode.AutoFlush,
            CacheEvictionPolicy = LogEntryCacheEvictionPolicy.OnSnapshot,
            WriteCounter = new("WAL.Writes", source),
            ReadCounter = new("WAL.Reads", source),
            CommitCounter = new("WAL.Commits", source),
            CompactionCounter = new("WAL.Compaction", source),
            LockContentionCounter = new("WAL.LockContention", source),
            LockDurationCounter = new("WAL.LockDuration", source),
        };

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

    BigStruct ISupplier<BigStruct>.Invoke() => content;

    private async ValueTask UpdateValue(LogEntry entry)
    {
        content = await entry.ToTypeAsync<BigStruct, LogEntry>().ConfigureAwait(false);
        //Console.WriteLine($"Accepting value");
    }

    protected override ValueTask ApplyAsync(LogEntry entry)
        => entry.Length == 0L ? new ValueTask() : UpdateValue(entry);

    protected override SnapshotBuilder CreateSnapshotBuilder(in SnapshotBuilderContext context)
    {
        //Console.WriteLine("Building snapshot");
        return new SimpleSnapshotBuilder(context);
    }
}