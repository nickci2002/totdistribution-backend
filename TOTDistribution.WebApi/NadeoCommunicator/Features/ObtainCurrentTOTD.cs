using ManiaAPI.NadeoAPI;
using TOTDistribution.Shared;
using TOTDistribution.Shared.Endpoints;

public static class ObtainCurrentTOTD
{
    // communicates with Nadeo API to obtain current TOTD
    public record Request;
    public record Response
    {
        public MapGuid MapId { get; init; }
        public string MapUid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public PlayerGuid Author { get; init; }
        public PlayerGuid Submitter { get; init; }
        public DateTimeOffset UploadTimestamp { get; init; }
    };

    //TrackOfTheDay TrackOfTheDay { get; init; }
    //MapInfoLive MapInfoLive { get; init; }

    //public record Invalid

    public sealed class Endpoint: IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/current-totd", Handler);
        }
    }

    public static async Task<IResult> Handler(
        NadeoLiveServices liveServices,
        CancellationToken cancellationToken)
    {
        var totds = await liveServices.GetTrackOfTheDaysAsync(1, 0, false, cancellationToken);
        var totd = totds.MonthList[0].Days[DateTime.UtcNow.Day - 1];

        if (totd is null)
        {
            return Results.NotFound();
        }

        var mapInfo = await liveServices.GetMapInfoAsync(totd!.MapUid, cancellationToken);

        Response response = new Response
        {
            MapId = mapInfo!.MapId,
            MapUid = mapInfo.Uid,
            Name = mapInfo.Name,
            Author = mapInfo.Author,
            Submitter = mapInfo.Submitter,
            UploadTimestamp = mapInfo.UploadTimestamp
        };

        return Results.Ok(response);
    }
}