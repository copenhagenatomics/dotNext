using DotNext.Net.Cluster.Consensus.Raft;
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


        AsyncWriter.WriteLine($"Running leader server on port {port}");
            
        var leadershipToken = cluster.LeadershipToken;

        using (var server = new ResponseSocket())
        {
            server.Bind($"tcp://*:{port}");
            while (!stoppingToken.IsCancellationRequested)
            {
                byte[] messageBytes = server.ReceiveFrameBytes(out bool more);

                //AsyncWriter.WriteLine($"Leader:\tReceived {messageBytes}, more = {more}, replicating");
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
                    var replymsg = replicateSucces ? $"good {replicationIndex}" : "fail";

                    //AsyncWriter.WriteLine($"Leader:\treplication {replymsg} after {stopWatch.ElapsedMilliseconds} ms. Sending reply");
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
    }


    private async Task<long> ClusterStartReplicate(byte[] data, CancellationToken stoppingToken = default)
    {
        var entry = new ByteArrayLogEntry(data, cluster.Term, 2);
        return await cluster.AuditTrail.AppendAsync(entry);
    }
    private async Task<bool> ClusterReplicate(byte[] data, CancellationToken stoppingToken = default)
    {
        var entry = new ByteArrayLogEntry(data, cluster.Term, 2);
        return await cluster.ReplicateAsync(entry, stoppingToken);
    }
}