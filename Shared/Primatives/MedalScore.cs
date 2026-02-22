using System.Text.Json.Serialization;
using TmEssentials;
using TOTDBackend.Shared.JsonConverters;

namespace TOTDBackend.Shared;

[JsonConverter(typeof(PrimitiveConverter<MedalScore, int>))]
public readonly record struct MedalScore(int Value) : IPrimitiveType<int>
{
    public static implicit operator MedalScore(TimeInt32 value) =>
        new() { Value = value.TotalMilliseconds };

    public override string ToString() => $"{Value}";
}


