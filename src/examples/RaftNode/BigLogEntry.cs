using DotNext.IO;
using DotNext.IO.Log;
using DotNext.Net.Cluster.Consensus.Raft;

namespace RaftNode;

internal sealed class BigLogEntry : BinaryTransferObject<BigStruct>, IRaftLogEntry
{
    internal BigLogEntry()
    {
        Timestamp = DateTimeOffset.UtcNow;
    }

    bool ILogEntry.IsSnapshot => false;

    public long Term { get; set; }

    public DateTimeOffset Timestamp { get; }
}

internal unsafe struct BigStruct
{
    public fixed byte Buffer[8000];
}
