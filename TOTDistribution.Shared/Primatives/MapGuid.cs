using System.Text.Json;
using System.Text.Json.Serialization;

namespace TOTDistribution.Shared;

public readonly record struct MapGuid
{
    public Guid Value { get; init; }

    public static implicit operator MapGuid(Guid value) => new MapGuid { Value = value };
    public static implicit operator MapGuid(Guid? value) => new MapGuid { Value = value ?? Guid.Empty };
}

public class MapGuidConverter : JsonConverter<MapGuid>
{
    public override MapGuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Guid value = reader.GetGuid();
        return new MapGuid { Value = value };
    }

    public override void Write(Utf8JsonWriter writer, MapGuid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
