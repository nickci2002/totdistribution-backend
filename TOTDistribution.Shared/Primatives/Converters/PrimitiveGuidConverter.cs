using System.Text.Json;
using System.Text.Json.Serialization;

namespace TOTDistribution.Shared.JsonConverters;

public class PrimitiveGuidConverter<T> : JsonConverter<T>
    where T : IPrimitiveType<Guid>, new()
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(T).IsAssignableFrom(typeToConvert);
    }
    
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = CanConvert(typeToConvert) ? reader.GetGuid() : Guid.Empty;
        return new T { Value = value };
    }


    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
