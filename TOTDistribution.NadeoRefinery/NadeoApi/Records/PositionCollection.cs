using System.Collections.Immutable;

namespace TOTDistribution.NadeoRefinery.NadeoApi.Records;

public sealed record PositionCollection(ImmutableList<Position> Positions);