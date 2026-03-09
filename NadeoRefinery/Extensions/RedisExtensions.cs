using System.Text.Json;
using Microsoft.Extensions.Options;
using Redis.OM;
using Redis.OM.Searching;
using StackExchange.Redis;
using TOTDBackend.NadeoRefinery.Models.Entities;
using TOTDBackend.Shared.Json;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class RedisExtensions
{
    public static IServiceCollection AddRedisDbServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSingleton(sp => {
            var redisConnString = config.GetSection("Redis").GetValue<string>("CM_ConnectionString")!;
            return ConnectionMultiplexer.Connect(redisConnString);
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