using ManiaAPI.NadeoAPI;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using TOTDistribution.NadeoRefinery.NadeoApi.Records;

namespace TOTDistribution.NadeoRefinery.NadeoApi;

[JsonSerializable(typeof(MapGroupIdCollection))]
[JsonSerializable(typeof(ImmutableList<MapPosition>))]
[JsonSerializable(typeof(TopLeaderboardCollection))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class ExtendedJsonContext : JsonSerializerContext;