using System.Text.Json;
using TOTDBackend.Shared.Json;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class JsonExtensions
{
    public static IServiceCollection ConfigureJsonProperties(this IServiceCollection services)
    {
        var converter = new CustomPrimitiveConverterFactory();

        services.AddSingleton(sp =>
        {
            var serializer = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            serializer.Converters.Add(converter);

            return serializer;
        });

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(converter);
        });
        
        return services;
    }
}