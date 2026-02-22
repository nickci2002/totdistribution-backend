using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace TOTDBackend.Shared.RabbitMQ;

public abstract class RabbitMqServiceBase(
    ILogger logger,
    string queueName)
    : BackgroundService
{
    protected IConnection? Connection { get; private set; }
    protected IChannel? Channel { get; private set; }

    private readonly ConnectionFactory _factory = new()
    { 
        HostName = "localhost",
        AutomaticRecoveryEnabled = true,
        TopologyRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
    };
    private readonly CreateChannelOptions _channelOpts = new(
        publisherConfirmationsEnabled: true,
        publisherConfirmationTrackingEnabled: true
    );
    protected readonly string QueueName = queueName ?? "queue-name";

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {type} RabbitMQ service...", GetType().Name);

        Connection = await _factory.CreateConnectionAsync(cancellationToken);
        
        Connection.ConnectionShutdownAsync += (_, args) =>
        {
            logger.LogWarning("RabbitMQ connection shutdown: {Reason}", args.ReplyText);
            return Task.CompletedTask;
        };

        Connection.RecoverySucceededAsync += (_, _) =>
        {
            logger.LogInformation("RabbitMQ recovery succeeded.");
            return Task.CompletedTask;
        };

        Connection.ConnectionRecoveryErrorAsync += (_, args) =>
        {
            logger.LogError(args.Exception, "RabbitMQ recovery failed.");
            return Task.CompletedTask;
        };

        Channel = await Connection.CreateChannelAsync(
            options: _channelOpts,
            cancellationToken: cancellationToken);

        await ConfigureChannelAsync(Channel, cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    protected virtual Task ConfigureChannelAsync(
        IChannel channel,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping {type} RabbitMQ service...", GetType().Name);

        if (Channel is not null)
            await Channel.DisposeAsync();

        if (Connection is not null)
            await Connection.DisposeAsync();

        await base.StopAsync(cancellationToken);
    }
}