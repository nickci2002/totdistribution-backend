using TOTDBackend.Shared.Json;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class JsonExtensions
{
    public static IServiceCollection ConfigureJsonProperties(this IServiceCollection services) =>
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new CustomPrimitiveConverterFactory()));
}