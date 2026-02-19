using ManiaAPI.NadeoAPI;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using TOTDBackend.NadeoRefinery.NadeoApi.Records;

namespace TOTDBackend.NadeoRefinery.NadeoApi;

[JsonSerializable(typeof(MapGroupIdCollection))]
[JsonSerializable(typeof(ImmutableList<Position>))]
[JsonSerializable(typeof(TopLeaderboardCollection))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class ExtendedJsonContext : JsonSerializerContext;