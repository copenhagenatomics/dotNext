using System.Collections.Concurrent;

using System.Diagnostics;

namespace RaftNode;
    public static class AsyncWriter 
    {

        
        private static BlockingCollection<string?> blockingCollection = new BlockingCollection<string?>();

        static AsyncWriter()  
        {  
          var thread = new Thread(
          () =>
          {
              while (true)  
              {  
                System.Console.Write(blockingCollection.Take());  
                
              }  
  
          });  
              thread.IsBackground = true;
    thread.Start();
      }  
      public static void WriteLine(string? value)  
      {  
        
        blockingCollection.Add(value + '\n');  
      }  

        public static void Write(string? value)
      {  
          
          blockingCollection.Add(value);  
      } 


      public static void performanceTest()
      {
                //run print test
        Console.WriteLine("running test of async console write");
        Thread.Sleep(1000);
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        for (int i = 0; i < 1000; i++)
        {
            Console.WriteLine("***********************Teststring of 80 characters********************Cons******");
        }
        stopWatch.Stop();


        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        Thread.Sleep(10000);
        // Format and display the TimeSpan value.
        string elapsedTimeConsole = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds);


        stopWatch.Restart();
        for (int i = 0; i < 1000; i++)
        {
            AsyncWriter.WriteLine("***********************Teststring of 80 characters*******************Async******");
        }
        stopWatch.Stop();
        Thread.Sleep(10000);

        // Get the elapsed time as a TimeSpan value.
        ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTimeAsyncprint = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds);
        Console.WriteLine("Console time for 1000*80 characters: " + elapsedTimeConsole);

        Console.WriteLine("Async time for 1000*80 characters: " + elapsedTimeAsyncprint);

      /**
      Results:
      ssh:
        Console time for 1000*80 characters: 00:00:00.55
        Async time for 1000*80 characters: 00:00:00.01

      local dotnet run:
        Console time for 1000*80 characters: 00:00:00.58
        Async time for 1000*80 characters: 00:00:00.00
      
      Serial (115200 baud):
        Console time for 1000*80 characters: 00:00:07.122
        Async time for 1000*80 characters: 00:00:00.01
    **/
      }
    }