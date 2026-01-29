using System.Text.Json;
using System.Text.Json.Serialization;

namespace TOTDistribution.Shared;

public readonly record struct PlayerGuid
{
    public Guid Value { get; init; }
    
    public static implicit operator PlayerGuid(Guid value) => new PlayerGuid { Value = value };
}

public class PlayerGuidConverter : JsonConverter<PlayerGuid>
{
    public override PlayerGuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Guid value = reader.GetGuid();
        return new PlayerGuid { Value = value };
    }

    public override void Write(Utf8JsonWriter writer, PlayerGuid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
