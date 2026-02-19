using System.Collections.Immutable;

namespace TOTDBackend.NadeoRefinery.NadeoApi.Records;

public sealed record MapGroupIdCollection(ImmutableList<MapGroupId> Maps);