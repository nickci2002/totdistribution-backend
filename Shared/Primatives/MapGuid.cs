using System.Text.Json.Serialization;
using TOTDBackend.Shared.JsonConverters;

namespace TOTDBackend.Shared;

[JsonConverter(typeof(PrimitiveConverter<MapGuid, Guid>))]
public readonly record struct MapGuid(Guid Value) : IPrimitiveType<Guid>
{
    public static implicit operator MapGuid(Guid value) => new() { Value = value };
    public static implicit operator MapGuid(Guid? value) => new() { Value = value ?? Guid.Empty };
}
