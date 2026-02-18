using System.Text.Json.Serialization;
using TOTDistribution.Shared.JsonConverters;

namespace TOTDistribution.Shared;

[JsonConverter(typeof(PrimitiveConverter<MapUid, string>))]
public readonly record struct MapUid : IPrimitiveType<string>
{
    public string Value { get; init; }

    public static implicit operator MapUid(string value) => new() { Value = value ?? string.Empty };
}
