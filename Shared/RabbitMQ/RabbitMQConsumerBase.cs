using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace TOTDBackend.Shared.RabbitMQ;

public class RabbitMqConsumerBase(
    ILogger<RabbitMqConsumerBase> logger,
    string queueName)
    : RabbitMqServiceBase(logger, queueName)
{
    protected override async Task ConfigureChannelAsync(IChannel channel, CancellationToken cancellationToken)
    {
        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 10,
            global: false,
            cancellationToken: cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (Channel is null)
            throw new InvalidOperationException("Channel not initialized.");

        var consumer = new AsyncEventingBasicConsumer(Channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());

                logger.LogInformation("Received: {Message}", message);

                await Channel.BasicAckAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);
            }
            catch
            {
                await Channel.BasicNackAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: cancellationToken);
            }
        };

        await Channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}