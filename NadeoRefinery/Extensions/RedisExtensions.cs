using StackExchange.Redis;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class RedisExtensions
{
    public static IServiceCollection AddRedisDbServices(
        this IServiceCollection services,
        IConnectionMultiplexer multiplexer)
    {
        services.AddSingleton(sp => multiplexer);

        return services;
    }
}