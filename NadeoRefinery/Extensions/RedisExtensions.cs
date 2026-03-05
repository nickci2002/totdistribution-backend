using Redis.OM;
using StackExchange.Redis;
using TOTDBackend.NadeoRefinery.Models.Entities;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class RedisExtensions
{
    public static IServiceCollection AddRedisDbServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        var redisConnString = config.GetValue<string>("CM_ConnectionString")!;
        var multiplexer = ConnectionMultiplexer.Connect(redisConnString);

        services.AddSingleton<IConnectionMultiplexer>(sp => multiplexer);
        services.AddSingleton(sp => 
        {
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
            return new RedisConnectionProvider(multiplexer);
        });

        // services.AddSingleton(sp =>
        // {
        //     var redisConnString = config.GetValue<string>("RCP_ConnectionString")!;
        //     return new RedisConnectionProvider(redisConnString);
        // });

        services.AddHostedService<IndexCreationService>();

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

