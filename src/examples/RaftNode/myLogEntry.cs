using DotNext.IO;
using DotNext.IO.Log;
using DotNext.Net.Cluster.Consensus.Raft;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using DotNext.Text;


namespace RaftNode
{
public readonly struct MyLogEntry :  IRaftLogEntry
{


 bool IDataTransferObject.IsReusable => true;
  public const int Prefix = 1;

  private readonly Dictionary<string, double>? payload;

  public MyLogEntry(Dictionary<string, double>? Payload, long term)
  {
    Term = term;
    Timestamp = DateTimeOffset.Now;
    payload = Payload; //is not null? payload : new Dictionary<string, string>(0);
  }

  public MyLogEntry(string key, double value, long term)
  {
    Term = term;
    Timestamp = DateTimeOffset.Now;
    payload = new Dictionary<string, double>(1); //is not null? payload : new Dictionary<string, string>(0);
    payload.Add(key, value);
  }

  public long Term { get; }
  public DateTimeOffset Timestamp { get; }

    long? IDataTransferObject.Length // optional implementation, may return null
  {
    get
    {
      if (payload is null)
      {
        return null;
      }

      // compute length of the serialized data, in bytes
      long result = 4; //sizeof(payload.Count); 4 bytes for count

      foreach (var (key, value) in payload)
      {
        result += Encoding.ASCII.GetByteCount(key) + sizeof(double);
      }
      Console.WriteLine($"Get length of payload (count = {payload.Count}");
      return result;
    }
  }

  /*
  Method for writing log entry
  */
  public async ValueTask WriteToAsync<TWriter>(TWriter writer, CancellationToken token)
    where TWriter : notnull, IAsyncBinaryWriter
  {
    // write prefix to recognize this type of the log entry
    await writer.WriteInt16Async(Prefix, true, token);
    // write the number of entries
   if (payload is null)
   {
    await writer.WriteInt32Async(0, true, token);
   }
   else
   {
    await writer.WriteInt32Async(payload.Count, true, token);
    // write the entries
    //var context = new EncodingContext(Encoding.UTF8, true);
    foreach (var (key, value) in payload)
    {
      await writer.WriteAsync(Encoding.ASCII.GetBytes(key), LengthFormat.Plain, token);
      await writer.WriteAsync<double>(value, token);
    }
   }
  }

public static async ValueTask<Dictionary<string, double>?> TransformAsync<TReader>(TReader reader, CancellationToken token)
    where TReader : notnull, IAsyncBinaryReader
  {
    var count = await reader.ReadInt32Async(true, token);
    if (count == 0)
    {
      return null;
    }
    var result = new Dictionary<string, double>(count);
    // deserialize entries
    var context = new DecodingContext(Encoding.ASCII, true);
    while (count-- > 0)
    {
       string key = await reader.ReadStringAsync(LengthFormat.Plain, context, token);
       double value = await reader.ReadAsync<double>(token);
       
       result.Add(key, value);
    }
    return result;
}
/*
public static async ValueTask<Dictionary<string, string>> DecodeAsync<TReader>(TReader reader, CancellationToken token)
    where TReader : notnull, IAsyncBinaryReader
  {
    var count = await reader.ReadInt32Async(true, token);
    var result = new Dictionary<string, string>(count);
    // deserialize entries
    var context = new DecodingContext(Encoding.UTF8, true);
    while (count-- > 0)
    {
       string key = await reader.ReadStringAsync(LengthFormat.Plain, context, token);
       string value = await reader.ReadStringAsync(LengthFormat.Plain, context, token);
       result.Add(key, value);
    }
    return result;
}
*/


}



}