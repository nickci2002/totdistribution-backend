using Redis.OM;
using StackExchange.Redis;
using TOTDBackend.NadeoRefinery.Entities;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class RedisServices
{
    public static IServiceCollection AddRedisDb(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddHostedService<IndexCreationService>();
        
        // services.AddSingleton(sp =>
        // {
        //     var redisConnString = config.GetValue<string>("ConnectionString")!;
        //     return ConnectionMultiplexer.Connect(redisConnString);
        // });

        // services.AddSingleton(sp =>
        // {
        //     var multiplexer = sp.GetRequiredService<ConnectionMultiplexer>();
        //     return new RedisConnectionProvider(multiplexer);
        // });

        services.AddSingleton(sp =>
        {
            var redisConnString = config.GetValue<string>("ConnectionString")!;
            return new RedisConnectionProvider(redisConnString);
        });

        return services;
    }

    /// <summary>
    /// Helper service to create indeces for our RedisDB
    /// </summary>
    internal class IndexCreationService(RedisConnectionProvider provider) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await provider.Connection.CreateIndexAsync(typeof(TOTDInfo));
            await provider.Connection.CreateIndexAsync(typeof(Distribution));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

