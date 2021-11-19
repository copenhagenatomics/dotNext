using DotNext.Net.Cluster.Consensus.Raft;
using System;

using DotNext.Net.Cluster.Consensus.Raft.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RaftNode
{

sealed class MyMetricsCollector : MetricsCollector
{
    //static int reportInterval = 100;
    //static int index = 0;
    //static double[] broadCastTimes = new double[reportInterval];

    public override void MovedToLeaderState()
    {
        base.MovedToLeaderState();
        AsyncWriter.WriteLine("Moved to Leader State");
    }

    public override void MovedToCandidateState()
    {
        base.MovedToCandidateState();
        AsyncWriter.WriteLine("Moved to Candidate State");
    }

    public override void MovedToFollowerState()
    {
        base.MovedToFollowerState();
        AsyncWriter.WriteLine("Moved to Follower State");
    }

    public override void ReportHeartbeat()
    {
        AsyncWriter.Write(".");
    }

	public override void ReportBroadcastTime(TimeSpan value)
    {
        //AsyncWriter.WriteLine(value.Milliseconds.ToString());
        /*
        broadCastTimes[index] = value.TotalMilliseconds;
        index++;
        if (index > reportInterval)
        {
            double min = 1000000;
            double max = 0;
            double sum = 0;
            double avg = 0;

            foreach (var item in broadCastTimes)
            {
                min = item < min ? item : min;
                max = item > max ? item : max;
                sum += item;
            }
            if (sum > 0)
            {
                avg = sum / reportInterval;
            }
            else
            {
                avg = 0;
            }
            

            AsyncWriter.WriteLine($"Broadcast report: completed {reportInterval} Broadcasts: max: {max}, min: {min}, avg: {avg}");
            index = 0;
        }
        */
		//report broadcast time measured during sending the request to all cluster members
    }
}
}