using System.Text.Json.Serialization;
using TOTDBackend.Shared.Json;

namespace TOTDBackend.Shared.Primatives;

[JsonConverter(typeof(PrimitiveJsonConverter<PlayerGuid, Guid>))]
public readonly record struct PlayerGuid(Guid Value) : IPrimitiveType<Guid>
{
    public static implicit operator PlayerGuid(Guid? guid) => new() { Value = guid ?? Guid.Empty };
    public static implicit operator Guid(PlayerGuid? guid) => 
        guid is not null ? guid.Value.Value : Guid.Empty;
}