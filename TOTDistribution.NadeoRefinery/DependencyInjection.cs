using Serilog;
using TOTDistribution.NadeoRefinery.Extensions;
using TOTDistribution.Shared.JsonConverters;

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
            options.SerializerOptions.Converters.Add(new CustomPrimitiveConverterFactory()));

        services.AddRedisDb(config.GetSection("Redis"));
        services.AddNadeoAPIServices(config.GetSection("NadeoAPI"));
    }

    public static void AddHost(this IHostBuilder host)
    {
        host.AddSerilog();
    }
}