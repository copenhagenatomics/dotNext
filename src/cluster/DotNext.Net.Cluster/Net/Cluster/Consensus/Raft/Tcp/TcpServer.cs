using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using EndOfStreamException = System.IO.EndOfStreamException;

namespace DotNext.Net.Cluster.Consensus.Raft.Tcp;

using Buffers;
using TransportServices;

internal sealed class TcpServer : TcpTransport, IServer
{
    private enum ExchangeResult : byte
    {
        Success = 0,
        SocketError,
        TimeOut,
        Stopped,
    }

    private sealed class ServerNetworkStream : PacketStream
    {
        internal ServerNetworkStream(Socket client, bool useSsl)
            : base(client, true, useSsl)
        {
        }

        internal Task Authenticate(SslServerAuthenticationOptions options, CancellationToken token)
            => ssl is null ? Task.CompletedTask : ssl.AuthenticateAsServerAsync(options, token);

        internal async Task<ExchangeResult> Exchange(IExchange exchange, Memory<byte> buffer, TimeSpan receiveTimeout, CancellationToken token)
        {
            var result = ExchangeResult.Success;
            CancellationTokenSource? timeoutTracker = null;
            try
            {
                var (headers, request) = await ReadPacket(buffer, token).ConfigureAwait(false);
                timeoutTracker = CancellationTokenSource.CreateLinkedTokenSource(token);
                timeoutTracker.CancelAfter(receiveTimeout);
                while (await exchange.ProcessInboundMessageAsync(headers, request, timeoutTracker.Token).ConfigureAwait(false))
                {
                    bool waitForInput;
                    int count;
                    (headers, count, waitForInput) = await exchange.CreateOutboundMessageAsync(AdjustToPayload(buffer), timeoutTracker.Token).ConfigureAwait(false);

                    // transmit packet to the remote endpoint
                    await WritePacket(headers, buffer, count, timeoutTracker.Token).ConfigureAwait(false);
                    if (!waitForInput)
                        break;

                    // read response
                    (headers, request) = await ReadPacket(buffer, timeoutTracker.Token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException e)
            {
                exchange.OnCanceled(e.CancellationToken);
                result = timeoutTracker is null || token.IsCancellationRequested ? ExchangeResult.Stopped : ExchangeResult.TimeOut;
            }
            catch (Exception e) when (e is EndOfStreamException || e is SocketException || e.InnerException is SocketException)
            {
                exchange.OnException(e);
                result = ExchangeResult.SocketError;
                //Console.WriteLine(e);
            }
            catch (Exception e)
            {
                exchange.OnException(e);
            }
            finally
            {
                timeoutTracker?.Dispose();
            }

            return result;
        }
    }

    private readonly Socket socket;
    private readonly int backlog;
    private readonly Func<IReusableExchange> exchangeFactory;
    private readonly CancellationTokenSource transmissionState;
    private readonly CancellationToken lifecycleToken;
    private TimeSpan receiveTimeout;
    private volatile int connections;
    internal int GracefulShutdownTimeout;

    internal TcpServer(IPEndPoint address, int backlog, MemoryAllocator<byte> allocator, Func<IReusableExchange> exchangeFactory, ILoggerFactory loggerFactory)
        : base(address, allocator, loggerFactory)
    {
        socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        this.backlog = backlog;
        this.exchangeFactory = exchangeFactory;
        transmissionState = new CancellationTokenSource();
        lifecycleToken = transmissionState.Token; // cache token here to avoid ObjectDisposedException in HandleConnection
    }

    public TimeSpan ReceiveTimeout
    {
        get => receiveTimeout;
        set
        {
            socket.ReceiveTimeout = (int)value.TotalMilliseconds;
            receiveTimeout = value;
        }
    }

    internal SslServerAuthenticationOptions? SslOptions
    {
        get;
        set;
    }

    private async void HandleConnection(Socket remoteClient)
    {
        var sslOptions = SslOptions;
        var stream = new ServerNetworkStream(remoteClient, sslOptions is not null);
        var buffer = AllocTransmissionBlock();
        var exchange = exchangeFactory();
        Interlocked.Increment(ref connections);
        try
        {
            if (sslOptions is not null)
                await stream.Authenticate(sslOptions, lifecycleToken).ConfigureAwait(false);

            while (stream.Connected && !IsDisposed)
            {
                switch (await stream.Exchange(exchange, buffer.Memory, receiveTimeout, lifecycleToken).ConfigureAwait(false))
                {
                    default:
                        return;
                    case ExchangeResult.Success:
                        exchange.Reset();
                        continue;
                    case ExchangeResult.TimeOut:
                        await remoteClient.DisconnectAsync(false, lifecycleToken).ConfigureAwait(false);
                        logger.RequestTimedOut();
                        goto default;
                }
            }
        }
        catch (Exception e)
        {
            exchange.OnException(e);
        }
        finally
        {
            buffer.Dispose();
            stream.Close(GracefulShutdownTimeout);
            stream.Dispose();
            Interlocked.Decrement(ref connections);
        }
    }

    private async void Listen()
    {
        for (var pending = true; pending && !IsDisposed;)
        {
            try
            {
                var remoteClient = await socket.AcceptAsync(lifecycleToken).ConfigureAwait(false);
                ConfigureSocket(remoteClient, LingerOption, Ttl);
                ThreadPool.QueueUserWorkItem(HandleConnection, remoteClient, false);
            }
            catch (ObjectDisposedException)
            {
                pending = false;
            }
            catch (SocketException e)
            {
                switch (e.SocketErrorCode)
                {
                    case SocketError.OperationAborted:
                    case SocketError.ConnectionAborted:
                    case SocketError.Shutdown:
                        break;
                    default:
                        logger.SockerErrorOccurred(e.SocketErrorCode);
                        break;
                }

                pending = false;
            }
            catch (Exception e)
            {
                logger.SocketAcceptLoopTerminated(e);
                pending = false;
            }
        }
    }

    public void Start()
    {
        socket.Bind(Address);
        socket.Listen(backlog);
        Listen();
    }

    private bool NoMoreConnections() => connections <= 0;

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            try
            {
                if (!transmissionState.IsCancellationRequested)
                    transmissionState.Cancel(false);
            }
            finally
            {
                transmissionState.Dispose();
                socket.Dispose();
            }

            if (!SpinWait.SpinUntil(NoMoreConnections, GracefulShutdownTimeout))
                logger.TcpGracefulShutdownFailed(GracefulShutdownTimeout);
        }
    }
}