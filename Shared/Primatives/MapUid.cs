using System.Text.Json.Serialization;
using TOTDBackend.Shared.Json;

namespace TOTDBackend.Shared.Primatives;

[JsonConverter(typeof(PrimitiveJsonConverter<MapUid, string>))]
public readonly record struct MapUid(string Value) : IPrimitiveType<string>
{
    public static implicit operator MapUid(string? value) => new() { Value = value ?? string.Empty };
    public static implicit operator string(MapUid? value) => value ?? string.Empty;
}