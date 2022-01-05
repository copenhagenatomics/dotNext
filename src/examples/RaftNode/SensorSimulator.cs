using DotNext;
using DotNext.Net.Cluster.Consensus.Raft;
using DotNext.Threading;
using System.Diagnostics;
using NetMQ;
using NetMQ.Sockets;
using System.Net;

namespace RaftNode;






internal sealed class SensorSimulator
{
    private SensorDataReplicator replicator;
    private int delay_ms;
   
    public SensorSimulator(SensorDataReplicator Replicator, int cycleInterval_ms)
    {
        replicator = Replicator;
        delay_ms = cycleInterval_ms;
    }
    public void RunThread(CancellationToken cancellationToken)
    {
        var thread = new Thread(
        () =>
        {
            AsyncWriter.WriteLine("starting sensorDataSimulator");
            int index = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(delay_ms);
                simulateSensorCycle(index++);
            }
        });  
        thread.IsBackground = true;
        thread.Start();
    }
    
    private void simulateSensorCycle(int index)
    {
        var data = new sensorData(){
                entryType = 1, //Sensordata type
                magicvalA = 0x0f0f0f0f,
                index = index,
                magicvalB = 0x5555AAAA
        };

        //AsyncWriter.WriteLine($"sensorSim:\tCreated sensorData index {data.index}");

        replicator.QueueReplication(data);
    }
}