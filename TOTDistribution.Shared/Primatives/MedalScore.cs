using System.Text.Json.Serialization;
using TmEssentials;
using TOTDistribution.Shared.JsonConverters;

namespace TOTDistribution.Shared;

[JsonConverter(typeof(PrimitiveConverter<MedalScore, int>))]
public readonly record struct MedalScore : IPrimitiveType<int>
{
    public int Value { get; init; }

    public static implicit operator MedalScore(TimeInt32 value) =>
        new() { Value = value.TotalMilliseconds };

    public override string ToString() => $"{Value}";
}


