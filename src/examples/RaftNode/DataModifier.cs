using DotNext;
using DotNext.Net.Cluster.Consensus.Raft;
using DotNext.Threading;
using System.Diagnostics;

namespace RaftNode;


internal sealed class DataModifier : BackgroundService
{
    private readonly IRaftCluster cluster;
    private readonly IKValueProvider valueProvider;

    private readonly int entrySize;

    private readonly int entryN;

    public DataModifier(IRaftCluster cluster, IKValueProvider provider, int EntrySize, int EntryN)
    {
        this.cluster = cluster;
        valueProvider = provider;
        entrySize = EntrySize;
        entryN = EntryN;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Stopwatch stopWatch = new Stopwatch();
        long txSum = 0;
        int cycleNumber = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(400, stoppingToken).ConfigureAwait(false);

            var leadershipToken = cluster.LeadershipToken;
            //AsyncWriter.WriteLine($"LeadershipToken = {leadershipToken.IsCancellationRequested}");

            if (!leadershipToken.IsCancellationRequested)
            {   
                if (cycleNumber == 50)
                {
                    txSum = 0;
                }
                cycleNumber++;
                
                var cycleNumber_bytes = BitConverter.GetBytes(cycleNumber);

                var Data = new byte[entrySize];

                for (int i = 0; i < entrySize; i++)
                    {
                        Data[i] = BitConverter.GetBytes(i)[0];//; //cycleNumber_bytes[i % 4];
                    }
                

                var entry = new ByteArrayLogEntry(Data, cluster.Term);
                //AsyncWriter.Write(entry.ToString());

                var source = stoppingToken.LinkTo(leadershipToken);
                try
                {   
                    
                    
                    
                    //var entry = new Int64LogEntry { Content = newValue, Term = cluster.Term };
                    stopWatch.Restart();
                    var result = true;
                    //int i;

                    result = await cluster.ReplicateMultipleAsync(entry, entryN, stoppingToken);
                    /*
                    for (i = 0; i<nReplicas; i++)
                    {
                        result = await cluster.ReplicateAsync(entry, stoppingToken);
                        if (!result)
                        {
                            break;
                        }
                    }
                    */
                    
                    stopWatch.Stop();
                    txSum+=stopWatch.ElapsedMilliseconds;

                    //show average if first 50 samples are collected
                    //this is to disregard initial instability
                    if (cycleNumber > 50)
                    {
                        AsyncWriter.WriteLine($"Repliscated {entrySize} bytes {entryN} times in {stopWatch.ElapsedMilliseconds} ms. average over {cycleNumber-50}: {txSum/(cycleNumber-50)} ms result: {result}");
                    }
                    else
                    {
                        AsyncWriter.WriteLine($"Repliscated {entrySize} bytes {entryN} times in {stopWatch.ElapsedMilliseconds} ms. replicated {cycleNumber} times. result: {result}");
                    }

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