using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using DotNext.Net.Cluster.Consensus.Raft;
using DotNext.Threading;
using NetMQ;
using NetMQ.Sockets;
using System.Net;
using System.Diagnostics;


namespace RaftNode;

internal sealed class decisionLogic
{
private static BlockingCollection<LogEntryContent> unhandledEntries = new BlockingCollection<LogEntryContent>();
private static List<LogEntryContent> HandledEntries = new List<LogEntryContent>();

public controllerData latestResult;

public IRaftCluster cluster {get; set;}
public object ResultLock = new object();
public long LatestDescIndex = -1;

 public validationServer? decisionValidator;
public decisionLogic()
{
    decisionValidator = null;
    latestResult = new controllerData();
    latestResult.index = -1;
    latestResult.hashValue = "null";
}

    public bool CommitHandler(LogEntryContent CommitedEntry)
    {
        return unhandledEntries.TryAdd(CommitedEntry);
    }

    public void RunThread(CancellationToken cancellationToken)
    {
                var thread = new Thread(
          () =>
          {
            AsyncWriter.WriteLine("starting decisionLogic");
            while (!cancellationToken.IsCancellationRequested)  
            {  
                HandleEntry(unhandledEntries.Take());
            }  
  
          });  
              thread.IsBackground = true;
        thread.Start();
    }

    public void HandleEntry(LogEntryContent entry)
    {
        var entryType = entry.content[0];
        //AsyncWriter.Write("Taking entry type: ");

        if (HandledEntries.Count > 100)
        {
            HandledEntries.RemoveAt(0);
        }
        HandledEntries.Add(entry);



        switch (entryType)
        {
        case 1: //SensorData
            //AsyncWriter.WriteLine("SensorData");
            break;
        case 2: //D! command
            
            DecisionLogicWrapper(entry.index);
            break;
        default: //error
            AsyncWriter.WriteLine("Error");
            break;

        }
    }

    



    private bool DecisionLogicWrapper(long index)
    {
        //AsyncWriter.Write("D!: ");
        var hash = DecisionLogicCore();
        lock (ResultLock)
        {
            latestResult.index = index;
            latestResult.hashValue = hash;
        }
        
        

        //if leader
        if (!cluster.LeadershipToken.IsCancellationRequested)
        {
            if (decisionValidator is not null)
            {
                decisionValidator.prepareElection(index, hash);
            }
            
        }
        else
        {
            //send logic to leader
            SendToLeader(latestResult);
        }

        return true;
    }

    private bool SendToLeader(controllerData data)
    {

        //var databytes = controllerData.getBytes(data);
        var destination = GetLeaderDest();
        if (destination is null)
        {
            return false;
        }
        AsyncWriter.WriteLine($"dec logic:\tSending data index: '{data.index}' to leader {destination}");
        using (var requestSocket = new RequestSocket($">tcp://{destination}"))
        {
            if (!requestSocket.TrySendFrame(System.TimeSpan.FromMilliseconds(50), $"{data.hashValue}:{data.index}", false))
            {
                AsyncWriter.WriteLine("dec logic:\tFailed to send frame");
                return false;
            }
            if (!requestSocket.TryReceiveFrameString( System.TimeSpan.FromMilliseconds(50), System.Text.Encoding.ASCII, out var reply, out var more))
            {
                AsyncWriter.WriteLine("dec logic:\tReply timeout");
                return false;
            }

            switch (reply)
            {
                case "valid":
                    AsyncWriter.WriteLine("dec logic:\tsucces");
                    return true;
                default:
                    AsyncWriter.WriteLine($"dec logic:\treply: {reply}");
                    return false;

            }
        /*
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();


           
        stopWatch.Stop();
        */
        //AsyncWriter.WriteLine($"replicator:\tDone Sending index {data.index} after {stopWatch.ElapsedMilliseconds} ms");

        }
    }


    private string? GetLeaderDest()
    {
    if (cluster.Leader != null)
    {
        //get leader
        IPEndPoint leader = (IPEndPoint)cluster.Leader.EndPoint; //cast as ip end point
        
        //change leader address to have sensor port
        var port = leader.Port + 50; //TODO: make proper port config
        var address = leader.Address;
        return $"{leader.Address.ToString()}:{port.ToString()}";

        }
    else
    {
        AsyncWriter.WriteLine("dec logic:\tLeader unknown");
        return null;
    }
    }
    private string DecisionLogicCore()
    {
        var decisionEntry = HandledEntries.Last();

        var decisionTime = decisionEntry.timestamp;

        //AsyncWriter.WriteLine($"Last entry:Index = {decisionEntry.index}, entry time = {decisionEntry.timestamp.ToString()}");

        //filter out old entries
        var usableEntries = HandledEntries.Where(n => (decisionTime - n.timestamp).TotalMilliseconds < 2000);
        
        foreach (var entry in usableEntries)
        {
            //AsyncWriter.WriteLine($"Index = {entry.index}, entry time = {entry.timestamp.ToString()}, diff = {(decisionTime - entry.timestamp).TotalMilliseconds} ms");
        }

        //accumulate payload data
        var accumulatedData = concatData(usableEntries);

        //check if empty
        if (accumulatedData is null)
        {
            return "null";
        }

        // Create a SHA256   
        using (SHA256 sha256Hash = SHA256.Create())  
        {  
            // ComputeHash - returns byte array  
            byte[] myHash = sha256Hash.ComputeHash(accumulatedData);  

        

        // Convert byte array to a string   
        StringBuilder builder = new StringBuilder();  
        for (int i = 0; i < myHash.Length; i++)  
        {  
            builder.Append(myHash[i].ToString("x2"));  
        }  

        AsyncWriter.WriteLine($"D!: hash from {usableEntries.Count()} = builder.ToString()");

        return builder.ToString();

        }
    }


    private byte[]? concatData(IEnumerable<LogEntryContent>? entries)
    {
        var result = new byte[entries.Sum(a => a.content.Length)];

        int offset = 0;
        foreach (var entry in entries)
        {
            entry.content.CopyTo(result, offset);
            offset += entry.content.Length;
        }

        return result;

    }
}