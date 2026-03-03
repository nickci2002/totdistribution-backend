using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TOTDBackend.Shared.RabbitMQ.Features;

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
            var routingKey = args.RoutingKey;
            
            await using var scope = serviceProvider.CreateAsyncScope();
            var consumerHandler = scope.ServiceProvider
                .GetRequiredKeyedService<IRabbitMQConsumerHandlerComponent<TData>>(routingKey);

            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var payload = Deserialize(message);

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Received: {Message}", message);
                }

                await Channel.BasicAckAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);

                await consumerHandler.HandleConsumerAckAsync();
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

                await consumerHandler.HandleConsumerNackAsync();
            }
        };

        await Channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);
    }

    private TData Deserialize(string message)
    {
        return JsonSerializer.Deserialize<TData>(message, serializerOptions) ?? 
            throw new Exception("Error deserializing for RabbitMQConsumerBase!");
    }
}