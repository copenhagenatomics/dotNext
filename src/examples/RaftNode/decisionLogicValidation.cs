using DotNext.Net.Cluster.Consensus.Raft;
using System.Diagnostics;
using NetMQ;
using NetMQ.Sockets;

namespace RaftNode;

internal sealed class validationServer
{
    private readonly IRaftCluster cluster;
    //private readonly IKValueProvider valueProvider;
    private readonly int port;

    private int failcntDown = 200;
    private int votes = 0; //vote for yourself
    private bool voteComplete = false;
    private int rejects = 0;
    private long ElectionIndex = 0;
    private string localHash = "";
    private Object electionLock = new object();

    public validationServer(IRaftCluster cluster, int port)
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
            AsyncWriter.WriteLine("starting validation server");
            Serve(cancellationToken).GetAwaiter().GetResult();
        });  
        thread.IsBackground = true;
        thread.Start();
    }

    
    public void prepareElection(long index, string hash)
    {
        lock (electionLock)
        {

            votes = 1; //vote for yourself
            voteComplete = false;
            rejects = 0;
            ElectionIndex = index;
            localHash = hash;
            failcntDown--;
            if (failcntDown == 0)
            {
                failcntDown = 200;
                localHash = "errorhash";
            }
        }
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
                string decMessage = server.ReceiveFrameString(out bool more);
            
                //var followerData = controllerData.fromBytes(messageBytes);
                
                long msgIndex = 0;
                string msgHash = "";
                bool msgValid  = false;
                string reply = "";

                try 
                {
                    string[] words = decMessage.Split(':');
                    msgHash = words[0];
                    msgIndex = long.Parse(words[1]);
                    msgValid = true;
                }
                catch (Exception e)
                {
                    AsyncWriter.WriteLine($"Unexpected error {e}");
                    msgValid = false;
                    reply = "invalid";
                }


                AsyncWriter.WriteLine($"Leader:\tReceived hash{msgHash}, index = {msgIndex}"); 
                //interpret message 
                
                //check if election index is ok
                lock (electionLock)
                {
                    if (msgIndex != ElectionIndex)
                    {
                        msgValid = false;
                        reply = "invalid";
                        AsyncWriter.WriteLine($"Leader: election index mismatch l: {ElectionIndex}, f: {msgIndex}");
                        
                    }
                    else if (msgHash == localHash)
                    {
                        msgValid = true;
                        reply = "valid";
                        votes++;
                        AsyncWriter.WriteLine($"Leader: hash match\t {votes} votes, {rejects} rejects");
                        
                    }
                    else 
                    {
                        msgValid = false;
                        reply = "invalid";
                        rejects++;
                        AsyncWriter.WriteLine($"Leader: hash mismatch\t {votes} votes, {rejects} rejects");
                    }
                }
                //check if leader
                if (cluster.LeadershipToken.IsCancellationRequested)
                {
                    msgValid = false;
                    reply = "not leader";
                    AsyncWriter.WriteLine("err, recieved cmd while not leader");
                }
                
                //Reply instantly
                server.SendFrame(reply, false);

                

                //ensure that own calculation is ready

                //check if hash matches
                if (votes >= cluster.Members.Count/2)
                {
                    if (!voteComplete)
                    AsyncWriter.WriteLine("validation complete");
                    voteComplete = true;
                }
                if (rejects > cluster.Members.Count/2)
                {
                    if (!voteComplete)
                    AsyncWriter.WriteLine("rejection complete");
                    voteComplete = true;
                    throw new InvalidOperationException("Logfile cannot be read-only");
                }           
            }
        }
    }
}