using System.Text.Json;

namespace TOTDBackend.Shared.Common.RabbitMQ;

public interface IRabbitMQPublisherHandler : IRabbitMQComponentHandler
{
    public Task OnPublishSuccessAsync();
    public Task OnPublishFailureAsync();
}


public interface IRabbitMQPublisherHandler<TData> : IRabbitMQPublisherHandler
{
    public new TData Data { get; protected set; }

    public new TData Deserialize(string message, JsonSerializerOptions serializerContext) =>
        JsonSerializer.Deserialize<TData>(message, serializerContext) ?? 
            throw new Exception("Error deserializing for RabbitMQConsumerHandler");
}