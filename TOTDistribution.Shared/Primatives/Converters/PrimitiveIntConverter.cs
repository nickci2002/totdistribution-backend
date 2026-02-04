using System.Text.Json;
using System.Text.Json.Serialization;

namespace TOTDistribution.Shared.JsonConverters;

public class PrimitiveIntConverter<T> : JsonConverter<T>
    where T : IPrimitiveType<int>, new()
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(T).IsAssignableFrom(typeToConvert);
    }
    
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = CanConvert(typeToConvert) ? reader.GetInt32() : 0;
        return new T { Value = value };
    }


    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
