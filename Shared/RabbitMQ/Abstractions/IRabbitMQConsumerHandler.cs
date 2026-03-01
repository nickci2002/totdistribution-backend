using System.Text.Json;

namespace TOTDBackend.Shared.Common.RabbitMQ;

public interface IRabbitMQConsumerHandler : IRabbitMQComponentHandler
{
    public Task OnConsumerAckAsync();
    public Task OnConsumerNackAsync();
}

public interface IRabbitMQConsumerHandler<TData> : IRabbitMQConsumerHandler
{
    public new TData Data { get; protected set; }

    public new TData Deserialize(string message, JsonSerializerOptions serializerContext) =>
        JsonSerializer.Deserialize<TData>(message, serializerContext) ?? 
            throw new Exception("Error deserializing for RabbitMQConsumerHandler");
}