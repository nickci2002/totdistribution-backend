using System.Text.Json.Serialization;
using TOTDBackend.Shared.Json;

namespace TOTDBackend.Shared.Primatives;

[JsonConverter(typeof(PrimitiveJsonConverter<MapGuid, Guid>))]
public readonly record struct MapGuid(Guid Value) : IPrimitiveType<Guid>
{
    public static implicit operator MapGuid(Guid value) => new() { Value = value };
    public static implicit operator MapGuid(Guid? value) => new() { Value = value ?? Guid.Empty };
}
