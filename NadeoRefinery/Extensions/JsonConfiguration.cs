using TOTDBackend.Shared.Json;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class JsonConfigurationExtensions
{
    public static IServiceCollection ConfigureJsonProperties(this IServiceCollection services) =>
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new CustomPrimitiveConverterFactory()));
}