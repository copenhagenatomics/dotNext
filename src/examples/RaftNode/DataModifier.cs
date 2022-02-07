using DotNext;
using DotNext.IO;
using DotNext.IO.Log;
using DotNext.Net.Cluster.Consensus.Raft;

namespace RaftNode;

internal sealed class DataModifier : BackgroundService
{
    private readonly IRaftCluster cluster;

    public DataModifier(IRaftCluster cluster, ISupplier<byte[]> provider)
    {
        this.cluster = cluster;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool resuming = true;
        while (!stoppingToken.IsCancellationRequested)
        {
            var leadershipToken = cluster.LeadershipToken;
            if (!leadershipToken.IsCancellationRequested)
            {
                if (resuming)
                    Console.WriteLine("Resumed as leader");
                resuming = false;

                var source = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, leadershipToken);
                var linkedToken = source.Token;
                try
                {
                    var entry = new MyLogEntry { Content = new byte[8000], Term = cluster.Term };
                    await cluster.ReplicateAsync(entry, linkedToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected error {0}", e);
                    resuming = true;
                }
                finally
                {
                    source?.Dispose();
                }
            }
            else
            {
                resuming = true;
            }
        }
    }

    internal sealed class MyLogEntry : IRaftLogEntry
    {
        internal MyLogEntry()
        {
            Timestamp = DateTimeOffset.UtcNow;
        }

        bool ILogEntry.IsSnapshot => false;

        public long Term { get; set; }

        public DateTimeOffset Timestamp { get; }

        public bool IsReusable => false;

        public long? Length => Content.Length;

        public byte[] Content { get; set; } = Array.Empty<byte>();

        public ValueTask WriteToAsync<TWriter>(TWriter writer, CancellationToken token) where TWriter : IAsyncBinaryWriter => 
            writer.WriteAsync(Content, null, token);
    }
}