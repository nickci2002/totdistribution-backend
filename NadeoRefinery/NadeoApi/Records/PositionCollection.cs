using System.Collections.Immutable;

namespace TOTDBackend.NadeoRefinery.NadeoApi.Records;

public sealed record PositionCollection(ImmutableList<Position> Positions);