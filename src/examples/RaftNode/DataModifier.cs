using DotNext;
using DotNext.Net.Cluster.Consensus.Raft;
using DotNext.Threading;
using System.Diagnostics;

namespace RaftNode;

internal sealed class DataModifier : BackgroundService
{
    private readonly IRaftCluster cluster;
    private readonly IKValueProvider valueProvider;
    private readonly validationServer decisionValidator;

    public DataModifier(IRaftCluster cluster, IKValueProvider provider, validationServer validator)
    {
        this.cluster = cluster;
        valueProvider = provider;
        decisionValidator = validator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Stopwatch stopWatch = new Stopwatch();
        long txSum = 0;
        int cycleNumber = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(700, stoppingToken).ConfigureAwait(false);

            var leadershipToken = cluster.LeadershipToken;
            //AsyncWriter.WriteLine($"LeadershipToken = {leadershipToken.IsCancellationRequested}");

            if (!leadershipToken.IsCancellationRequested)
            {   
                cycleNumber++;

                var cycleNumber_bytes = BitConverter.GetBytes(cycleNumber);
                var payloadSize= 10;
                var nReplicas = 1;
                var Data = new byte[payloadSize];

                Data[0] = 2; //D! type
                for (int i = 1; i < payloadSize; i++)
                    {
                        Data[i] = BitConverter.GetBytes(i)[0];//; //cycleNumber_bytes[i % 4];
                    }
                

                var entry = new ByteArrayLogEntry(Data, cluster.Term, 2);
                //AsyncWriter.Write(entry.ToString());

                var source = stoppingToken.LinkTo(leadershipToken);
                try
                {   
                    
                    
                    
                    //var entry = new Int64LogEntry { Content = newValue, Term = cluster.Term };
                    stopWatch.Restart();
                    var result = true;
                    //int i;

                    result = await cluster.ReplicateMultipleAsync(entry, nReplicas, stoppingToken);

                    
                    stopWatch.Stop();
                    txSum+=stopWatch.ElapsedMilliseconds;

                    AsyncWriter.WriteLine($"Replicated {payloadSize} bytes {nReplicas} times in {stopWatch.ElapsedMilliseconds} ms. average over {cycleNumber}: {txSum/cycleNumber} ms result: {result}");

                }
                catch (Exception e)
                {
                    AsyncWriter.WriteLine($"Unexpected error {e}");
                }
                finally
                {
                    source?.Dispose();
                }
            }
        }
    }
}