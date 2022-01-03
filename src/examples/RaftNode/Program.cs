using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using DotNext.Net.Cluster;
using DotNext.Net.Cluster.Consensus.Raft;
using DotNext.Net.Cluster.Consensus.Raft.Http;
using DotNext.Net.Cluster.Consensus.Raft.Membership;
using Microsoft.Extensions.Logging.Console;
using RaftNode;
using SslOptions = DotNext.Net.Security.SslOptions;




AsyncWriter.WriteLine("starting application");



switch (args.LongLength)
{
    case 0:
    case 1:
        AsyncWriter.performanceTest();
        AsyncWriter.WriteLine("Port number and protocol are not specified");
        break;
    case 2:
        await StartNode(args[0], int.Parse(args[1]));
        break;
    case 3:
        await StartNode(args[0], int.Parse(args[1]), args[2]);
        break;
    case 4:
        await StartNode(args[0], int.Parse(args[1]), args[2], args[3]);
        break;
    
}

static Task UseAspNetCoreHost(int port, string? persistentStorage = null)
{
    var configuration = new Dictionary<string, string>
            {
                {"partitioning", "false"},
                {"lowerElectionTimeout", "150" },
                {"upperElectionTimeout", "300" },
                {"requestTimeout", "00:10:00"},
                {"publicEndPoint", $"https://localhost:{port}"},
                {"coldStart", "false"},
                {"requestJournal:memoryLimit", "5" },
                {"requestJournal:expiration", "00:01:00" }
            };
    if (!string.IsNullOrEmpty(persistentStorage))
        configuration[SimplePersistentState.LogLocation] = persistentStorage;
    return new HostBuilder().ConfigureWebHost(webHost =>
    {
        webHost.UseKestrel(options =>
        {
            options.ListenLocalhost(port, static listener => listener.UseHttps(LoadCertificate()));
        })
        .UseStartup<Startup>();
    })
    .ConfigureLogging(static builder => builder.AddConsole().SetMinimumLevel(LogLevel.Error))
    .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configuration))
    .JoinCluster()
    .Build()
    .RunAsync();
}

static async Task UseConfiguration(RaftCluster.NodeConfiguration config, string? persistentStorage, TestConfiguration? testCfg = null)
{
    AddMembersToCluster(config.UseInMemoryConfigurationStorage(), persistentStorage, testCfg);
    var loggerFactory = new LoggerFactory();
    var loggerOptions = new ConsoleLoggerOptions
    {
        LogToStandardErrorThreshold = LogLevel.Warning
    };
    loggerFactory.AddProvider(new ConsoleLoggerProvider(new FakeOptionsMonitor<ConsoleLoggerOptions>(loggerOptions)));
    config.LoggerFactory = loggerFactory;

    config.Metrics = new MyMetricsCollector();

    using var cluster = new RaftCluster(config);
    cluster.LeaderChanged += ClusterConfigurator.LeaderChanged;

    var modifier = default(BackgroundService?);
    var sensorSim = default(SensorSimulator?);
    var replicator = default(SensorDataReplicator?);
    var dataReciever = default(clientDataReceiver?);
    var logicMachine= new decisionLogic();

    var state = new SimplePersistentState(persistentStorage, new AppEventSource(), logicMachine.CommitHandler);
    cluster.AuditTrail = state;
    if (testCfg.sensorData)
    {
        replicator = new SensorDataReplicator(cluster, 10);
        sensorSim = new SensorSimulator(replicator, 1000);
        
        dataReciever = new clientDataReceiver(cluster, config.HostEndPoint.Port+100);
        modifier = new DataModifier(cluster, state);
    }
    else
    {
        modifier = new DataModifier(cluster, state);
    }
    await cluster.StartAsync(CancellationToken.None);

    if (testCfg.sensorData)
    {
        AsyncWriter.WriteLine("Starting message handling threads");
        replicator.RunThread(CancellationToken.None);
        sensorSim.RunThread(CancellationToken.None);
        dataReciever.RunThread(CancellationToken.None);
        logicMachine.RunThread(CancellationToken.None);
        AsyncWriter.WriteLine("done");
        await (modifier?.StartAsync(CancellationToken.None) ?? Task.CompletedTask);
    }
    else
    {
        await (modifier?.StartAsync(CancellationToken.None) ?? Task.CompletedTask);
    }

    using var handler = new CancelKeyPressHandler();
    Console.CancelKeyPress += handler.Handler;
    await handler.WaitAsync();
    Console.CancelKeyPress -= handler.Handler;
    await (modifier?.StopAsync(CancellationToken.None) ?? Task.CompletedTask);
    await cluster.StopAsync(CancellationToken.None);

    // NOTE: this way of adding members to the cluster is not recommended in production code
    static void AddMembersToCluster(InMemoryClusterConfigurationStorage<IPEndPoint> storage, string? persistentStorage, TestConfiguration? testCfg = null)
    {
        var builder = storage.CreateActiveConfigurationBuilder();
        
        if ((testCfg is not null))
        {
            AsyncWriter.WriteLine("creating member list");
            foreach (var node in testCfg.NodeList)
            {
                //use ip of endpoint
                var address = new IPEndPoint(new IPAddress(node.Value.ip_byte), node.Value.port);
                //var address = new IPEndPoint(IPAddress.Loopback, 3262);
                builder.Add(ClusterMemberId.FromEndPoint(address), address); 
                AsyncWriter.WriteLine($"Adding {node.Key} with address = {address}");
            }
        }

        builder.Build();
    }
}

static Task UseUdpTransport(int port, string? persistentStorage)
{
    var configuration = new RaftCluster.UdpConfiguration(new IPEndPoint(IPAddress.Loopback, port))
    {
        LowerElectionTimeout = 150,
        UpperElectionTimeout = 300,
        DatagramSize = 1024,
        ColdStart = false,
    };
    return UseConfiguration(configuration, persistentStorage);
}

static Task UseTcpTransport(int port, string? persistentStorage, bool useSsl, string? configPath = null)
{
    var myConfig = new ConfigurationParser(configPath).config;
    IPAddress ipaddr = new IPAddress(new byte[] { 0, 0, 0, 0 });
    
    //find ip of node from list
    foreach (var node in myConfig.NodeList)
    {
        if (node.Key == persistentStorage) 
        {
            ipaddr = new IPAddress(node.Value.ip_byte);
        }
    }

    AsyncWriter.WriteLine($"configuring cluster for node with ip: {ipaddr}");
    AsyncWriter.WriteLine($"lower = {myConfig.LowerElectionTimeout}, upper = {myConfig.UpperElectionTimeout}");

    var configuration = new RaftCluster.TcpConfiguration(new IPEndPoint(ipaddr, port))
    {
        ConnectTimeout = 50,
        LowerElectionTimeout = myConfig.LowerElectionTimeout,
        UpperElectionTimeout = myConfig.UpperElectionTimeout,
        TransmissionBlockSize = myConfig.TransmissionBlockSize,
        RequestTimeout = TimeSpan.FromMilliseconds(myConfig.UpperElectionTimeout / 3D),
        ColdStart = false,
        SslOptions = useSsl ? CreateSslOptions() : null
    };

    return UseConfiguration(configuration, persistentStorage, myConfig);

    static SslOptions CreateSslOptions()
    {
        var options = new SslOptions();
        options.ServerOptions.ServerCertificate = LoadCertificate();
        options.ClientOptions.TargetHost = "localhost";
        options.ClientOptions.RemoteCertificateValidationCallback = RaftClientHandlerFactory.AllowCertificate;
        return options;
    }
}

static Task StartNode(string protocol, int port, string? persistentStorage = null, string? configPath = null )
{
    switch (protocol.ToLowerInvariant())
    {
        case "http":
        case "https":
            return UseAspNetCoreHost(port, persistentStorage);
        case "udp":
            return UseUdpTransport(port, persistentStorage);
        case "tcp":
            return UseTcpTransport(port, persistentStorage, false, configPath);
        case "tcp+ssl":
            return UseTcpTransport(port, persistentStorage, true);
        default:
            Console.Error.WriteLine("Unsupported protocol type");
            Environment.ExitCode = 1;
            return Task.CompletedTask;
    }
}

static X509Certificate2 LoadCertificate()
{
    using var rawCertificate = Assembly.GetCallingAssembly().GetManifestResourceStream(typeof(Program), "node.pfx");
    using var ms = new MemoryStream(1024);
    rawCertificate?.CopyTo(ms);
    ms.Seek(0, SeekOrigin.Begin);
    return new X509Certificate2(ms.ToArray(), "1234");
}