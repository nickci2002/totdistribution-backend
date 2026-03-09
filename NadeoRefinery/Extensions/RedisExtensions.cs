using StackExchange.Redis;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class RedisExtensions
{
    public static IServiceCollection AddRedisDbServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp => {
            var redisConnString = config.GetSection("Redis").GetValue<string>("CM_ConnectionString")!;
            return ConnectionMultiplexer.Connect(redisConnString);
        });

        return services;
    }
}