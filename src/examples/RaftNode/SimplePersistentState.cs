using DotNext;
using DotNext.IO;
using DotNext.Net.Cluster.Consensus.Raft;
using static DotNext.Threading.AtomicInt64;

namespace RaftNode;


public class MyInterpreter : IDataTransferObject.ITransformation<int>
{

    public byte[]? Data;




/*
Called by infrastructure. uses MyLogEntry to transform entry to Dictionary object, stored in class.

return: entry prefix type <int>
*/
    public async ValueTask<int> TransformAsync<TReader>(TReader reader, CancellationToken token)
        where TReader : notnull, IAsyncBinaryReader
    {
        Int16 prefix = 0;
        try
        {
        prefix = await reader.ReadInt16Async(true, token);
        }
        catch (Exception e)
        {
            AsyncWriter.WriteLine($"Unexpected error {e}");
        }
        switch (prefix)
        {
            case ByteArrayLogEntry.Prefix:
            //Dict object cannot be returned. store in class.
                Data = await ByteArrayLogEntry.TransformAsync(reader, token);

        
            // interpretation logic here
            break;
            default:
            AsyncWriter.WriteLine($"Unknown prefix {prefix}");
            break;
    }
    return prefix;
    }

    public async ValueTask<byte[]> InterpretAsync<TEntry>(TEntry entry)
    where TEntry : struct, IRaftLogEntry
    {
        var Prefix = await entry.TransformAsync<int, MyInterpreter>(this, CancellationToken.None);
        
        return Data is not null ? Data : new byte[0];

        
    }

public int UpdateLocalState(ref byte[] data)
{
    data = Data != null ? Data : new byte[0];

    return 1;
    
}

    public int PrintState(ref byte[] data)
    {
        int idx = 0;
        AsyncWriter.WriteLine($"Data length = {data.Length}");

        while (idx < data.Length)
        {
            for (int i = 0; i<16; i++)
            {
                AsyncWriter.Write($"{data[idx]}\t");
            }
            AsyncWriter.WriteLine("");
        }

        return 1;
    }
}

internal sealed class SimplePersistentState : PersistentState, IKValueProvider//, ISupplier<long>
{
    internal const string LogLocation = "logLocation";

    private sealed class SimpleSnapshotBuilder : IncrementalSnapshotBuilder
    {
        private byte[] content;

        public SimpleSnapshotBuilder(in SnapshotBuilderContext context, MyInterpreter interpreter)
            : base(context)
        {
                Interpreter = interpreter;
                content = new byte[0];
        }

        private MyInterpreter Interpreter;

        protected override async ValueTask ApplyAsync(LogEntry entry)
        {
            if (entry.Length != 0)
            {

                if (entry.IsSnapshot)
                {
                    content = await Interpreter.InterpretAsync(entry);
                }
                else
                {
                    await Interpreter.InterpretAsync(entry);
                    //Interpreter.UpdateLocalState(ref content);
                }
            }
            else
            {
                new ValueTask();
            }
        }
           // => value = await entry.ToTypeAsync<long, LogEntry>().ConfigureAwait(false);

        public override ValueTask WriteToAsync<TWriter>(TWriter writer, CancellationToken token)
        {
            var entry = new ByteArrayLogEntry(content, 0);
            return entry.WriteToAsync(writer, token);
        }
            //=> writer.WriteAsync(value, token);
    }

    
    private volatile byte[] content;

    public SimplePersistentState(string path, AppEventSource source)
        : base(path, 50, CreateOptions(source))
    {
        content = new byte[0];
    }

    public SimplePersistentState(IConfiguration configuration, AppEventSource source)
        : this(configuration[LogLocation], source)
    {
        content = new byte[0];
    }

    private static Options CreateOptions(AppEventSource source)
    {
        var result = new Options
        {
            InitialPartitionSize = 50 * 8,
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

    byte[] IKValueProvider.Value => content;
   // long ISupplier<long>.Invoke() => content.VolatileRead();

/*
    private async ValueTask UpdateValue(LogEntry entry)
    {
        var value = await entry.ToTypeAsync<long, LogEntry>().ConfigureAwait(false);
        content.VolatileWrite(value);
        AsyncWriter.WriteLine($"Accepting value {value}");
    }
    */
    protected override async ValueTask ApplyAsync(LogEntry entry)
    {
        AsyncWriter.WriteLine($"ApplyAsync entry length = {entry.Length}");
        
        if (entry.Length != 0)
        {
            MyInterpreter interpreter = new MyInterpreter();
            
            if (entry.IsSnapshot)
            {
            // interpret snapshot
                AsyncWriter.WriteLine("Got Snapshot, overwritting content.");
                
                content = await interpreter.InterpretAsync(entry);
            }
            else
            {
                await interpreter.InterpretAsync(entry);
                AsyncWriter.WriteLine("applying entry to content");
                interpreter.UpdateLocalState(ref content);
            }
        }
        else
        {
            new ValueTask();
        }
    }
    protected override SnapshotBuilder CreateSnapshotBuilder(in SnapshotBuilderContext context)
    {
        AsyncWriter.WriteLine("Building snapshot");
        return new SimpleSnapshotBuilder(context, new MyInterpreter());
    }
}