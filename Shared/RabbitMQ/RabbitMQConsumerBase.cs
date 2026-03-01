using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TOTDBackend.Shared.Common.RabbitMQ;

namespace TOTDBackend.Shared.RabbitMQ;

public class RabbitMqConsumerBase<TData>(
    ILogger<RabbitMqConsumerBase<TData>> logger,
    IOptions<MessageQueueOptions> mqOptions,
    JsonSerializerOptions serializerOptions,
    IServiceProvider serviceProvider)
    : RabbitMQServiceBase(logger, mqOptions)
{
    protected override async Task InstantiateAsync(CancellationToken cancellationToken)
    {
        await base.InstantiateAsync(cancellationToken);

        await Channel!.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 10,
            global: false,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(Channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var consumerHandler = scope.ServiceProvider.GetRequiredService<IRabbitMQConsumerHandler<TData>>();

            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var payload = consumerHandler.Deserialize(message, serializerOptions);

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Received: {Message}", message);
                }

                await Channel.BasicAckAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);

                await consumerHandler.OnConsumerAckAsync();
            }
            catch
            {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError("Error consuming message! Retrying...");
                }

                await Channel.BasicNackAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: cancellationToken);

                await consumerHandler.OnConsumerNackAsync();
            }
        };

        await Channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);
    }
}