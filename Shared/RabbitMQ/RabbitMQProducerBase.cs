using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;

namespace TOTDBackend.Shared.RabbitMQ;

public class RabbitMqProducerBase(
    ILogger<RabbitMqProducerBase> logger,
    string queueName)
    : RabbitMqServiceBase(logger, queueName)
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (Channel is null)
            throw new InvalidOperationException("Channel not initialized.");

        while (!cancellationToken.IsCancellationRequested)
        {
            var message = $"Ping {DateTime.UtcNow}";
            var body = Encoding.UTF8.GetBytes(message);

            await Channel.BasicPublishAsync(
                exchange: "",
                routingKey: QueueName,
                body: body,
                cancellationToken: cancellationToken);

            logger.LogInformation("Published: {Message}", message);

            await Task.Delay(5000, cancellationToken);
        }
    }
}