using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using TmEssentials;

namespace TOTDistribution.Shared;

public readonly record struct MedalScore
{
    public int Value { get; init; }

    public static implicit operator MedalScore(TimeInt32 value) =>
        new MedalScore { Value = value.TotalMilliseconds };
}

public class MedalScoreConverter : JsonConverter<MedalScore>
{
    public override MedalScore Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(MedalScore));
        return new MedalScore{Value = reader.GetInt32()};
    }

    public override void Write(Utf8JsonWriter writer, MedalScore value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
