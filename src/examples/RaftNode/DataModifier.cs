﻿using DotNext;
using DotNext.Net.Cluster.Consensus.Raft;
using DotNext.Threading;
using System.Diagnostics;

namespace RaftNode;

internal sealed class DataModifier : BackgroundService
{
    private readonly IRaftCluster cluster;
    private readonly IKValueProvider valueProvider;

    public DataModifier(IRaftCluster cluster, IKValueProvider provider)
    {
        this.cluster = cluster;
        valueProvider = provider;
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
                cycleNumber++;

                var cycleNumber_bytes = BitConverter.GetBytes(cycleNumber);
                var payloadSize= 1024;
                var nReplicas = 16;
                var Data = new byte[payloadSize];

                for (int i = 0; i < payloadSize; i++)
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

                    result = await cluster.ReplicateMultipleAsync(entry, nReplicas, stoppingToken);
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