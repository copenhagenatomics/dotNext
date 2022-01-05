using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;


namespace RaftNode;

internal sealed class decisionLogic
{
private static BlockingCollection<LogEntryContent> unhandledEntries = new BlockingCollection<LogEntryContent>();
private static List<LogEntryContent> HandledEntries = new List<LogEntryContent>();
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
        AsyncWriter.Write("Taking entry type: ");

        if (HandledEntries.Count > 100)
        {
            HandledEntries.RemoveAt(0);
        }
        HandledEntries.Add(entry);



        switch (entryType)
        {
        case 1: //SensorData
            AsyncWriter.WriteLine("SensorData");
            break;
        case 2: //D! command
            AsyncWriter.WriteLine("D!");
            AsyncWriter.WriteLine($"logic HASH = {DecisionLogic()}");
            break;
        default: //error
            AsyncWriter.WriteLine("Error");
            break;

        }
    }

    private string DecisionLogic()
    {
        var decisionEntry = HandledEntries.Last();

        var decisionTime = decisionEntry.timestamp;

        AsyncWriter.WriteLine($"Last entry:Index = {decisionEntry.index}, entry time = {decisionEntry.timestamp.ToString()}");

        //filter out old entries
        var usableEntries = HandledEntries.Where(n => (decisionTime - n.timestamp).TotalMilliseconds < 2000);
        
        foreach (var entry in usableEntries)
        {
            AsyncWriter.WriteLine($"Index = {entry.index}, entry time = {entry.timestamp.ToString()}, diff = {(decisionTime - entry.timestamp).TotalMilliseconds} ms");
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