namespace TOTDBackend.Shared.RabbitMQ.Features;

public interface IRabbitMQConsumerHandlerComponent<TData>
{
    Task HandleConsumerAckAsync();
    Task HandleConsumerNackAsync();
}