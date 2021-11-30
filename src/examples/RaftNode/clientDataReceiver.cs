using DotNext;
using DotNext.Net.Cluster.Consensus.Raft;
using DotNext.Threading;
using System.Diagnostics;
using NetMQ;
using NetMQ.Sockets;

namespace RaftNode;

internal sealed class clientDataReceiver
{
    private readonly IRaftCluster cluster;
    //private readonly IKValueProvider valueProvider;
    private readonly int port;

    public clientDataReceiver(IRaftCluster cluster, int port)
    {
        this.cluster = cluster;
        //valueProvider = provider;
        this.port = port;
    }

    public void RunThread(CancellationToken cancellationToken)
    {
        //Task.Run() 
        
        var thread = new Thread(
        () =>
        {
            AsyncWriter.WriteLine("starting sensorData replicator");
            Serve(cancellationToken).GetAwaiter().GetResult();
        });  
        thread.IsBackground = true;
        thread.Start();
    }

    private async Task Serve(CancellationToken stoppingToken)
    {
        Stopwatch stopWatch = new Stopwatch();
        long txSum = 0;
        int cycleNumber = 0;

        AsyncWriter.WriteLine($"Running leader server on port {port}");
            
        var leadershipToken = cluster.LeadershipToken;

        using (var server = new ResponseSocket())
        {
            server.Bind($"tcp://*:{port}");
            while (!stoppingToken.IsCancellationRequested)
            {
                byte[] messageBytes = server.ReceiveFrameBytes(out bool more);

                AsyncWriter.WriteLine($"Leader:\tReceived {messageBytes}, more = {more}, replicating");
                //Reply instantly
                var message = sensorData.fromBytes(messageBytes);
                
                if (true) //!leadershipToken.IsCancellationRequested)
                {
                    server.SendFrame("replicating", true);
                    
                    var replicateSucces = false;
                    long replicationIndex = 0;
                    stopWatch.Restart();
                    try
                    {
                        replicationIndex = await ClusterStartReplicate(messageBytes, stoppingToken);
                        //replicateSucces = await ClusterReplicate(messageBytes, stoppingToken);
                        replicateSucces = true;
                    }
                    catch (Exception e)
                    {
                        AsyncWriter.WriteLine($"Unexpected error {e}");
                    }
                    
                    stopWatch.Stop();
                    var replymsg = replicateSucces ? $"good index {replicationIndex}" : "fail";

                    AsyncWriter.WriteLine($"Leader:\treplication {replymsg} after {stopWatch.ElapsedMilliseconds} ms. Sending reply");
                    if (!server.TrySendFrame(TimeSpan.FromMilliseconds(100), replymsg, false))
                    {
                        AsyncWriter.WriteLine("Leader:\tReply transmission failed");
                    }
                }
                else
                {
                    server.SendFrame("not leader", false);

                }
                

                //Append entry (no force commit)

            }
        }



            //AsyncWriter.WriteLine($"LeadershipToken = {leadershipToken.IsCancellationRequested}");
/*
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
                    
                    for (i = 0; i<nReplicas; i++)
                    {
                        result = await cluster.ReplicateAsync(entry, stoppingToken);
                        if (!result)
                        {
                            break;
                        }
                    }
                    
                    
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
            */
        
    }


    private async Task<long> ClusterStartReplicate(byte[] data, CancellationToken stoppingToken = default)
    {
        var entry = new ByteArrayLogEntry(data, cluster.Term);
        return await cluster.AuditTrail.AppendAsync(entry);
    }
    private async Task<bool> ClusterReplicate(byte[] data, CancellationToken stoppingToken = default)
    {
        var entry = new ByteArrayLogEntry(data, cluster.Term);
        return await cluster.ReplicateAsync(entry, stoppingToken);
    }
}