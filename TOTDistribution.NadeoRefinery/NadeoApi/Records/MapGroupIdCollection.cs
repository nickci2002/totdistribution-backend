using System.Collections.Immutable;

namespace TOTDistribution.NadeoRefinery.NadeoApi.Records;

public sealed record MapGroupIdCollection(ImmutableList<MapGroupId> Maps);