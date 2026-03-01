
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TOTDBackend.Shared.Json;

public sealed class CustomPrimitiveConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return GetCustomPrimitiveInterface(typeToConvert) != null;
    }

    public override JsonConverter CreateConverter(
        Type typeToConvert, JsonSerializerOptions options)
    {
        var primitiveInterface = GetCustomPrimitiveInterface(typeToConvert)!;
        var valueType = primitiveInterface.GetGenericArguments()[0];

        var converterType = typeof(PrimitiveJsonConverter<,>)
            .MakeGenericType(typeToConvert, valueType);

        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private static Type? GetCustomPrimitiveInterface(Type type)
    {
        return type
            .GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IPrimitiveType<>));
    }
}