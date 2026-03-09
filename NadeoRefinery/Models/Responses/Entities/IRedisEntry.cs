using StackExchange.Redis;

namespace TOTDBackend.NadeoRefinery.Models.Entities;

public interface IRedisEntity
{
    abstract static string? KeyPrefix { get; }
    
    public static sealed string GetKey<T>(string key) where T : IRedisEntity => $"{T.KeyPrefix}:{key}";
    HashEntry[] Hashify();
    IRedisEntity Dehashify(HashEntry[]? entries);
}