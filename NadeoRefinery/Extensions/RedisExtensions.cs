using StackExchange.Redis;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class RedisExtensions
{
    public static IConnectionMultiplexer? GetRedisMultiplexer(
        this IHostApplicationBuilder builder,
        IConfiguration config)
    {
        try {
            // For local dev
            return GetRedisMultiplexerFromConnectionString(
                config.GetSection("Redis").GetValue<string>("CM_ConnectionString")!);
        }
        catch
        {
            // For Aspire
            return GetRedisMultiplexerFromConnectionString(
                builder.Configuration.GetConnectionString("aspire-redis")!);
        }
    }

    private static IConnectionMultiplexer GetRedisMultiplexerFromConnectionString(string getConnectionString)
    {
        return ConnectionMultiplexer.Connect(getConnectionString);
    }

    public static IServiceCollection AddRedisDbServices(
        this IServiceCollection services,
        IConnectionMultiplexer multiplexer)
    {
        services.AddSingleton(sp => multiplexer);

        return services;
    }
}