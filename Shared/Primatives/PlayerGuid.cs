using System.Text.Json.Serialization;
using TOTDBackend.Shared.Json;

namespace TOTDBackend.Shared.Primatives;

[JsonConverter(typeof(PrimitiveJsonConverter<PlayerGuid, Guid>))]
public readonly record struct PlayerGuid(Guid Value) : IPrimitiveType<Guid>
{
    public static implicit operator PlayerGuid(Guid? value) => new() { Value = value ?? Guid.Empty };
    public static implicit operator Guid(PlayerGuid? value) => value ?? Guid.Empty;
}