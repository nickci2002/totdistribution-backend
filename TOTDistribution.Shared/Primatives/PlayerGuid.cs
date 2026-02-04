using System.Text.Json.Serialization;
using TOTDistribution.Shared.JsonConverters;

namespace TOTDistribution.Shared;

[JsonConverter(typeof(PrimitiveConverter<PlayerGuid, Guid>))]
public readonly record struct PlayerGuid : IPrimitiveType<Guid>
{
    public Guid Value { get; init; }
    
    public static implicit operator PlayerGuid(Guid value) => new() { Value = value };
    public static implicit operator PlayerGuid(Guid? value) => new() { Value = value ?? Guid.Empty };
    
}
