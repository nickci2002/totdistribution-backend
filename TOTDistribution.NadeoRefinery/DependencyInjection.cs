using Serilog;
using TOTDistribution.NadeoRefinery.Extensions;
using TOTDistribution.Shared;

namespace TOTDistribution.NadeoRefinery;

public static class DependencyInjection
{
    public static void AddNadeoRefinery(this IServiceCollection services)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("secrets.json")
            .AddEnvironmentVariables()
            .Build();

        services.ConfigureHttpJsonOptions(options =>
        {
            var converters = options.SerializerOptions.Converters;
            converters.Add(new MapGuidConverter());
            converters.Add(new MedalScoreConverter());
            converters.Add(new PlayerGuidConverter());

        });

        services.AddRedisDb(config.GetSection("Redis"));
        services.AddNadeoAPIServices(config.GetSection("NadeoAPI"));
    }

    public static void AddHost(this IHostBuilder host)
    {
        host.AddSerilog();
    }
}