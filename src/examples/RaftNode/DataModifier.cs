using DotNext;
using DotNext.Net.Cluster.Consensus.Raft;

namespace RaftNode;

internal sealed class DataModifier : BackgroundService
{
    private readonly IRaftCluster cluster;
    private readonly ISupplier<BigStruct> valueProvider;

    public DataModifier(IRaftCluster cluster, ISupplier<BigStruct> provider)
    {
        this.cluster = cluster;
        valueProvider = provider;
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
                    var entry = new BigLogEntry { Content = new BigStruct(), Term = cluster.Term };
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
}