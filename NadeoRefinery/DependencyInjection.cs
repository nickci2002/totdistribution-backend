using System.Reflection;
using Serilog;
using TOTDBackend.NadeoRefinery.Extensions;
using TOTDBackend.Shared.JsonConverters;

namespace TOTDBackend.NadeoRefinery;

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
        services.AddNadeoQuerySlices();
#if WEB_API
        services.AddTestingEndpoints(sliceTypes);
#endif
    }

    public static void AddHost(this IHostBuilder host)
    {
        host.AddSerilog();
    }
}