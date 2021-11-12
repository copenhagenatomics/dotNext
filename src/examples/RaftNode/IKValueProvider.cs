using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RaftNode
{
    internal interface IKValueProvider
    {
        Dictionary<string, double> Value { get; }
        
       // Task<bool> UpdateValueAsync(string key, string value, TimeSpan timeout, CancellationToken token);
    }
}
