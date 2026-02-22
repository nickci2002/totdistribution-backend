using System.Text.Json.Serialization;
using TOTDBackend.Shared.JsonConverters;

namespace TOTDBackend.Shared;

[JsonConverter(typeof(PrimitiveConverter<PlayerGuid, Guid>))]
public readonly record struct PlayerGuid(Guid Value) : IPrimitiveType<Guid>
{
    public static implicit operator PlayerGuid(Guid value) => new() { Value = value };
    public static implicit operator PlayerGuid(Guid? value) => new() { Value = value ?? Guid.Empty };
}
