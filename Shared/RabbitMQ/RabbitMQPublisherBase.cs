using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;
using TOTDBackend.Shared.RabbitMQ.Features;

namespace TOTDBackend.Shared.RabbitMQ;

public class RabbitMQPublisherBase<TData>(
    ILogger<RabbitMQPublisherBase<TData>> logger,
    IOptions<MessageQueueOptions> mqOptions,
    JsonSerializerOptions serializerOptions)
    : RabbitMQServiceBase(logger, mqOptions)
    where TData : notnull
{
    public async Task PublishAsync(
        TData data,
        IRabbitMQPublisherHandlerComponent<TData> handler,
        CancellationToken cancellationToken)
    {
        try
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Publishing {type} to RabbitMQ...", typeof(TData));
            }

            await AutoQueueDeclareAsync(cancellationToken);

            var bodyAsBytes = SerializeToUtf8Bytes(data);
            await Channel!.BasicPublishAsync(
                exchange: "",
                routingKey: QueueName,
                body: bodyAsBytes,
                cancellationToken: cancellationToken);
            
            await handler.HandlePublishConfirmAsync();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Message sent successfully!");
            }
        }
        catch
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError("There was an error sending the message!");
            }

            await handler.HandlePublishFailureAsync();
        }
    }

    private byte[] SerializeToUtf8Bytes(TData data)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data, data.GetType(), serializerOptions);
    }
}