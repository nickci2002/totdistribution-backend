using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TOTDistribution.NadeoRefinery.Entities;
using Redis.OM;

namespace TOTDistribution.NadeoRefinery.Data;

public static class RedisServices
{
    public static void AddRedisDb(this IServiceCollection services, IConfiguration config)
    {
        services.AddHostedService<IndexCreationService>();

        services.AddSingleton(sp =>
        {
            var redisConnString = config.GetValue<string>("ConnectionString")!;
            return new RedisConnectionProvider(redisConnString);
        });
    }

    /// <summary>
    /// Helper service to create indeces for our RedisDB
    /// </summary>
    internal class IndexCreationService : IHostedService
    {
        private readonly RedisConnectionProvider _provider;

        public IndexCreationService(RedisConnectionProvider provider)
        {
            _provider = provider;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _provider.Connection.CreateIndexAsync(typeof(TOTDInfo));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

