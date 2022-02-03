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
                    for (int i = 1; i <= 15; i++)
                    {
                        var entry = new BigLogEntry { Content = new BigStruct(), Term = cluster.Term };
                        await cluster.AuditTrail.AppendAsync(entry, linkedToken).ConfigureAwait(false);
                    }

                    var lastEntry = new BigLogEntry { Content = new BigStruct(), Term = cluster.Term };
                    var lastIndex = await cluster.AuditTrail.AppendAsync(lastEntry, linkedToken).ConfigureAwait(false);

                    await cluster.ForceReplicationAsync(linkedToken).ConfigureAwait(false);
                    await cluster.AuditTrail.WaitForCommitAsync(lastIndex, linkedToken).ConfigureAwait(false);
                    if (cluster.AuditTrail.Term != lastEntry.Term)
                        Console.WriteLine($"failed to replicate last entry");
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
                //note this timeout might actually be longer depending on the system clock resolution i.e. 15 ms in windows
                //we use WhenAny to make the TaskCancelledException silent if the stoppingToken is triggered
                await Task.WhenAny(Task.Delay(1, stoppingToken));
            }
        }
    }
}