using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace DotNext.Net.Cluster.Consensus.Raft.Commands
{
    using IO;

    /// <summary>
    /// Represents Raft log entry containing custom command.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct LogEntry<TCommand> : IRaftLogEntry
        where TCommand : struct
    {
        private readonly TCommand command;
        private readonly ICommandFormatter<TCommand> formatter;
        private readonly int id;

        internal LogEntry(long term, TCommand command, ICommandFormatter<TCommand> formatter, int id)
        {
            Term = term;
            Timestamp = DateTimeOffset.Now;
            this.command = command;
            this.formatter = formatter;
            this.id = id;
        }

        /// <inheritdoc />
        public long Term { get; }

        /// <inheritdoc />
        public DateTimeOffset Timestamp { get; }

        /// <inheritdoc />
        bool IDataTransferObject.IsReusable => true;

        /// <inheritdoc />
        long? IDataTransferObject.Length => formatter.GetLength(in command);

        /// <inheritdoc />
        async ValueTask IDataTransferObject.WriteToAsync<TWriter>(TWriter writer, CancellationToken token)
        {
            await writer.WriteInt32Async(id, true, token).ConfigureAwait(false);
            await formatter.SerializeAsync(command, writer, token).ConfigureAwait(false);
        }
    }
}