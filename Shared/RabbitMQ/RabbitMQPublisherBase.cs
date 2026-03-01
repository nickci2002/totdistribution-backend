using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;
using TOTDBackend.Shared.Common.RabbitMQ;

namespace TOTDBackend.Shared.RabbitMQ;

public class RabbitMQPublisherBase<TData>(
    ILogger<RabbitMQPublisherBase<TData>> logger,
    IOptions<MessageQueueOptions> mqOptions,
    JsonSerializerOptions serializerOptions,
    IServiceProvider serviceProvider)
    : RabbitMQServiceBase(logger, mqOptions)
{
    public async Task PublishAsync(TData body, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var publisherHandler = scope.ServiceProvider.GetRequiredService<IRabbitMQPublisherHandler<TData>>();

        try
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Publishing {type} to RabbitMQ...", typeof(TData));
            }

            await AutoQueueDeclareAsync(cancellationToken);

            var bodyAsBytes = JsonSerializer.SerializeToUtf8Bytes(body, serializerOptions);
            await Channel!.BasicPublishAsync(
                exchange: "",
                routingKey: QueueName,
                body: bodyAsBytes,
                cancellationToken: cancellationToken);
            
            await publisherHandler.OnPublishSuccessAsync();

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

            await publisherHandler.OnPublishFailureAsync();
        }
    }
}