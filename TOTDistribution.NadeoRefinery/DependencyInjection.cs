using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TOTDistribution.NadeoRefinery.Data;
using StackExchange.Redis;
using TOTDistribution.NadeoRefinery.Services;

namespace TOTDistribution.NadeoRefinery;

public static class DependencyInjection
{
    public static void AddNadeoRefinery(this IServiceCollection services)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("secrets.json")
            .AddEnvironmentVariables()
            .Build();

        services.AddRedisDb(config.GetSection("Redis"));
        services.AddNadeoAPIServices(config.GetSection("NadeoAPI"));

        // lol
    }

}
