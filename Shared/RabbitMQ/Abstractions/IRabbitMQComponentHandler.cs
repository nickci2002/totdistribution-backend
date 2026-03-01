using System.Text.Json;

namespace TOTDBackend.Shared.Common.RabbitMQ;

public interface IRabbitMQComponentHandler
{
    public object Data { get; protected set; }

    public byte[] SerializeToUtf8Bytes(JsonSerializerOptions serializerContext) =>
        JsonSerializer.SerializeToUtf8Bytes(Data, Data.GetType(), serializerContext);
    
    public object Deserialize(string message, JsonSerializerOptions serializerContext);
}