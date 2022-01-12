using DotNext;
using DotNext.Net.Cluster.Consensus.Raft;
using DotNext.Threading;
using System.Diagnostics;
using NetMQ;
using NetMQ.Sockets;
using System.Net;
using System.Collections.Concurrent;


namespace RaftNode;




internal sealed class SensorDataReplicator
{
    private readonly IRaftCluster cluster;
    

    private static BlockingCollection<sensorData> blockingCollection = new BlockingCollection<sensorData>();
    int queueSize;
    public SensorDataReplicator(IRaftCluster cluster, int maxSize)
    {
        this.cluster = cluster;
        queueSize = maxSize;
    }

    public void RunThread(CancellationToken cancellationToken)
    {
                var thread = new Thread(
          () =>
          {
            AsyncWriter.WriteLine("starting sensorData replicator");
            while (!cancellationToken.IsCancellationRequested)  
            {  
                replicate(blockingCollection.Take());
            }  
  
          });  
              thread.IsBackground = true;
    thread.Start();
    }
    

    public bool QueueReplication(sensorData data)
    {
        //Convert data to byte array
        
        //Throw away old elements when over filled
        while (blockingCollection.Count >= queueSize)
        {
            blockingCollection.Take();
        }

        blockingCollection.Add(data);  

        return true;
    }
     
    private void replicate(sensorData data)
    {
        if (cluster.Leader != null)
        {
            //get leader
            IPEndPoint leader = (IPEndPoint)cluster.Leader.EndPoint; //cast as ip end point
            
            //change leader address to have sensor port
            var port = leader.Port + 100; //TODO: make proper port config
            var address = leader.Address;
            var addrStr= $"{leader.Address.ToString()}:{port.ToString()}";
            
            var databytes = sensorData.getBytes(data);

           
            //AsyncWriter.WriteLine($"replicator:\tSending data index: '{data.index}' to leader {leader.Address.ToString()}:{port.ToString()}");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            SendSensorData(addrStr, databytes, 100);    
            stopWatch.Stop();
            
            //AsyncWriter.WriteLine($"replicator:\tDone Sending index {data.index} after {stopWatch.ElapsedMilliseconds} ms");

        }
        else
        {
            AsyncWriter.WriteLine("replicator:\tLeader unknown");
        }
    }

    /**
    *   message order
        sensornode -> Leader: (Sensordata)
        Leader -> Sensornode: ["replicating", "not leader", "bad data"] //TODO: change to codes
        if "replicating":
            sensornode wait for replication
            Leader -> SensorNode: ["succes", "fail"]
    *
    **/
    private bool SendSensorData(string address, byte[] Data, int timeout)
    {
        string reply = "";
        bool more = false;

        using (var requestSocket = new RequestSocket($">tcp://{address}"))
        {
             
            //AsyncWriter.WriteLine("replicator:\trequestSocket : Sending data");

            if (!requestSocket.TrySendFrame(System.TimeSpan.FromMilliseconds(timeout), Data, false))
            {
                AsyncWriter.WriteLine("replicator:\tFailed to send frame");
                return false;
            }
            if (!requestSocket.TryReceiveFrameString( System.TimeSpan.FromMilliseconds(timeout), System.Text.Encoding.ASCII, out reply, out more))
            {
                AsyncWriter.WriteLine("replicator:\tReply timeout");
                return false;
            }
            //AsyncWriter.WriteLine($"replicator:\trecieved reply: '{reply}'");



            switch (reply)
            {
                case "replicating":
                    if (!requestSocket.TryReceiveFrameString( System.TimeSpan.FromMilliseconds(timeout), System.Text.Encoding.ASCII, out reply, out more))
                    {
                        AsyncWriter.WriteLine("replicator:\tReply timeout");
                        return false;
                    }
                    
                    //AsyncWriter.WriteLine($"replicator:\treplication result: {reply}");
                    switch (reply.Substring(0, 4))
                    {
                        case "good":

                            if (Int32.TryParse(reply.Substring(4), out int index))
                            {
                                //AsyncWriter.WriteLine($"replicator:\tsucces index {index}");
                                cluster.AuditTrail.WaitForCommitAsync(index).ConfigureAwait(false);
                                //AsyncWriter.WriteLine($"replicator:\tcommit index {index}");
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("String could not be parsed.");
                                return false;
                            }
                            
                            
                        case "fail":
                        default:
                            AsyncWriter.WriteLine("replicator:\treply error");
                            return false;
                    }

                case "not leader":
                    return false;
                
                case "bad data":
                    return false;
                
                default:
                    return false;

            }
        }
    }
}