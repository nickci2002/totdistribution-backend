namespace TOTDBackend.Shared.RabbitMQ;

public class MessageQueueOptions
{
    public string QueueName { get; set; } = "queue-name";
    public int RetryCount { get; set; } = 5;
}