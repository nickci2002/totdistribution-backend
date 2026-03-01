using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace TOTDBackend.Shared.Extensions;

public static class JsonSerializerOptionsExtensions
{
    public static IServiceCollection AddJsonSerializerOptionsService(
        this IServiceCollection services, IEnumerable<JsonConverter> converters)
    {
        services.AddSingleton(sp =>
        {
            var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            return serializerOptions.AddJsonConvertersEnumerable(converters);
        });
        
        return services;
    }

    public static IServiceCollection ConfigureHttpJsonSerializerOptions(
        this IServiceCollection services, IEnumerable<JsonConverter> converters)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.AddJsonConvertersEnumerable(converters);
        });

        return services;
    }

    private static JsonSerializerOptions AddJsonConvertersEnumerable(
        this JsonSerializerOptions serializerOptions, IEnumerable<JsonConverter> converters)
    {
        foreach (var converter in converters)
        {
            serializerOptions.Converters.Add(converter);
        }

        return serializerOptions;
    }
}