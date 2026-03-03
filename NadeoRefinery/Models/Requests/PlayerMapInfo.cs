namespace TOTDBackend.NadeoRefinery.Models.Requests;

public readonly record struct PlayerMapInfo
{
    public Guid AccountId { get; init; }
    public Guid MapId { get; init; }
    public string GroupUid  { get; init; }
}