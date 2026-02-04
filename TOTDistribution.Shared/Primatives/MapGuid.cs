using System.Text.Json.Serialization;
using TOTDistribution.Shared.JsonConverters;

namespace TOTDistribution.Shared;

[JsonConverter(typeof(PrimitiveConverter<MapGuid, Guid>))]
public readonly record struct MapGuid : IPrimitiveType<Guid>
{
    public Guid Value { get; init; }

    public static implicit operator MapGuid(Guid value) => new() { Value = value };
    public static implicit operator MapGuid(Guid? value) => new() { Value = value ?? Guid.Empty };
}
