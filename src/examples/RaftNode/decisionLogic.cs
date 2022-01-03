using System.Collections.Concurrent;


namespace RaftNode;

internal sealed class decisionLogic
{
private static BlockingCollection<LogEntryContent> unhandledEntries = new BlockingCollection<LogEntryContent>();

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


        switch (entryType)
        {
        case 1: //SensorData
            AsyncWriter.WriteLine("SensorData");
            break;
        case 2: //D! command
            AsyncWriter.WriteLine("D!");
            break;
        default: //error
            AsyncWriter.WriteLine("Error");
            break;

        }
    }

}