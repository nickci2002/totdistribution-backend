using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace TOTDBackend.Shared.RabbitMQ;

public abstract class RabbitMQServiceBase(
    ILogger logger,
    IOptions<MessageQueueOptions> mqOptions)
    : IAsyncDisposable, IHostedService
{
    protected IConnection? Connection { get; private set; }
    protected IChannel? Channel { get; private set; }

    private readonly ConnectionFactory _factory = new() { 
        HostName = "localhost",
        AutomaticRecoveryEnabled = true,
        TopologyRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
    };

    private readonly CreateChannelOptions _channelOpts = new(
        publisherConfirmationsEnabled: true,
        publisherConfirmationTrackingEnabled: true
    );

    protected readonly string QueueName = mqOptions.Value.QueueName;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Factory.StartNew(async () =>
        {
            try
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Starting {type} RabbitMQ service...", GetType().Name);
                }

                await InstantiateAsync(cancellationToken);

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("RabbitMQ service {type} created.", GetType().Name);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error starting RabbitMQ connection!");
            }
        },
        TaskCreationOptions.LongRunning);

        return Task.CompletedTask;
    }

    protected virtual async Task InstantiateAsync(CancellationToken cancellationToken)
    {
        Connection = await _factory.CreateConnectionAsync(cancellationToken);
        
        Connection.ConnectionShutdownAsync += (_, args) =>
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning("RabbitMQ connection shutdown: {Reason}", args.ReplyText);
            }

            return Task.CompletedTask;
        };

        Connection.RecoverySucceededAsync += (_, _) =>
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("RabbitMQ recovery succeeded.");
            }

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

        await AutoQueueDeclareAsync(cancellationToken);
    }

    protected async Task AutoQueueDeclareAsync(CancellationToken cancellationToken)
    {
        await Channel!.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Disposing of {type} RabbitMQ service...", GetType().Name);
        }

        if (Channel is not null)
        {
            await Channel.DisposeAsync();
        }

        if (Connection is not null)
        {
            await Connection.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}