using System.Text.Json.Serialization;
using TOTDBackend.Shared.Json;

namespace TOTDBackend.Shared.Primatives;

[JsonConverter(typeof(PrimitiveJsonConverter<MapGuid, Guid>))]
public readonly record struct MapGuid(Guid Value) : IPrimitiveType<Guid>
{
    public static implicit operator MapGuid(Guid? value) => new() { Value = value ?? Guid.Empty };
    public static implicit operator Guid(MapGuid? guid) =>
        guid is not null ? guid.Value.Value : Guid.Empty;

    public override string ToString() => Value.ToString();
}