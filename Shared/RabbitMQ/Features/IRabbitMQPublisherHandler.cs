namespace TOTDBackend.Shared.RabbitMQ.Features;

public interface IRabbitMQPublisherHandlerComponent<TData>
{
    Task HandlePublishConfirmAsync();
    Task HandlePublishFailureAsync();
}
