using TOTDistribution.Shared;

namespace NadeoCommunicator.Features;

public static class CreateTrackData
{
    // communicates with Nadeo API to create track data
    public record Command
    {
        public MapGuid MapId { get; }
        public string MapUid { get; }
        public string Name { get; }
        public PlayerGuid Author { get; }
        public PlayerGuid Submitter { get; }
        public DateTime UploadTimestamp { get; }

        public Command(
            MapGuid mapId,
            string mapUid,
            string name,
            PlayerGuid author,
            PlayerGuid submitter,
            DateTime uploadTimestamp)
        {
            MapId = mapId;
            MapUid = mapUid;
            Name = name;
            Author = author;
            Submitter = submitter;
            UploadTimestamp = uploadTimestamp;
        }

        public static Command Create(
            MapGuid mapId,
            string mapUid,
            string name,
            PlayerGuid author,
            PlayerGuid submitter,
            DateTime uploadTimestamp)
        {
            return new Command(mapId, mapUid, name, author, submitter, uploadTimestamp);
        }
    }


}