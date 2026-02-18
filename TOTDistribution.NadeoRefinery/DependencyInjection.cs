using System.Diagnostics.Tracing;
using System.Reflection;
using Serilog;
using TOTDistribution.NadeoRefinery.Extensions;
using TOTDistribution.Shared.JsonConverters;

namespace TOTDistribution.NadeoRefinery;

public static class DependencyInjection
{
    public static void AddNadeoRefinery(this IServiceCollection services)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("secrets.json")
            .AddEnvironmentVariables()
            .Build();
        
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new CustomPrimitiveConverterFactory()));
        
        var sliceTypes = Assembly
            .GetExecutingAssembly()
            .DefinedTypes
            .Where((type) => type is { IsInterface: false, IsAbstract: false });
        
        services.AddRedisDb(config.GetSection("Redis"));
        services.AddNadeoAPIServices(config.GetSection("NadeoAPI"));
        services.AddNadeoQuerySlices(sliceTypes);
#if WEB_API
        services.AddTestingEndpoints(sliceTypes);
#endif
    }

    public static void AddHost(this IHostBuilder host)
    {
        host.AddSerilog();
    }
}