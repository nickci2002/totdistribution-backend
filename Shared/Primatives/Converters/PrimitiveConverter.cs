using System.Text.Json;
using System.Text.Json.Serialization;

namespace TOTDBackend.Shared.JsonConverters;

public class PrimitiveConverter<TPrimitive, TValue> : JsonConverter<TPrimitive>
    where TPrimitive : IPrimitiveType<TValue>, new()
{
    public override TPrimitive Read(ref Utf8JsonReader reader,
                                    Type typeToConvert,
                                    JsonSerializerOptions options)
    {
        TValue value = JsonSerializer.Deserialize<TValue>(ref reader, options)!;
        return new TPrimitive { Value = value };
    }

    public override void Write(Utf8JsonWriter writer,
                               TPrimitive value,
                               JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }
}