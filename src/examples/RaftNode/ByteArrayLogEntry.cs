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
public readonly struct ByteArrayLogEntry :  IRaftLogEntry
{


 bool IDataTransferObject.IsReusable => true;
  public const int Prefix = 2;

  //private readonly Dictionary<string, double>? payload;
  private readonly byte[] payload;
  public ByteArrayLogEntry(byte[] Payload, long term)
  {
    
    Term = term;
    Timestamp = DateTimeOffset.Now;
    payload = Payload; 
  }

  public long Term { get; }
  public DateTimeOffset Timestamp { get; }

    long? IDataTransferObject.Length // optional implementation, may return null
  { 
    get
    {

      long result = 4 + payload.Length + 4;

      AsyncWriter.WriteLine($"Get length of payload: {result}");
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
    
    // write the length
    //await writer.WriteInt32Async(payload.Length, true, token);
    
    
    // write the entries
    await writer.WriteAsync(payload, LengthFormat.Plain, token);

    await writer.WriteInt32Async(0x0FF0F0F0, true, token);

  }

public override string? ToString()
{
  string result = $"[byteArrayLogEntry of length: {payload.Length}\n";

          int idx = 0;
        AsyncWriter.WriteLine($"Data:\n");

        while (idx < payload.Length)
        {
            for (int i = 0; i<16; i++)
            {
                result += $"{payload[idx]}\t";
                idx++;
                if (!(idx < payload.Length))
                {
                  break;
                }
            }
            result += "\n";
        }
  result += "]\n";
  return result;
}



public static async ValueTask<byte[]> TransformAsync<TReader>(TReader reader, CancellationToken token)
    where TReader : notnull, IAsyncBinaryReader
  {

    var length = await reader.ReadInt32Async(true, token);
    
    var result = new byte[length];

    //read memory
    if (length > 0)
    {
      await reader.ReadAsync(result, token);
    }
    //read crc
    var chksum = await reader.ReadInt32Async(true, token);
    if (chksum != 0x0FF0F0F0)
    {
      AsyncWriter.WriteLine($"checksum error: 0x{chksum:X8}, length: {length}");
    }

    return result;
}



}



}