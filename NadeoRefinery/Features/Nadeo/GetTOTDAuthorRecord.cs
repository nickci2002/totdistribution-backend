using ManiaAPI.NadeoAPI;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Models.Entities;
using TOTDBackend.NadeoRefinery.Models.Requests;
using TOTDBackend.NadeoRefinery.Models.Responses;

namespace TOTDBackend.NadeoRefinery.Features.Nadeo;

/// <inheritdoc cref="NadeoCommunicatorSlice{TResp}" />
internal sealed class GetTOTDAuthorRecord(NadeoServices services)
    : NadeoCommunicatorSlice<PlayerMapInfo, PlayerScore>
{
    /// <inheritdoc cref="INadeoConsumerComponent{TReq, TResp}" />
    internal class Consumer(NadeoServices services)
        : INadeoConsumerComponent<PlayerMapInfo, PlayerScore>
    {
        public async Task<PlayerScore> FetchDataAsync(PlayerMapInfo request)
        {
            var accountId = request.AccountId;
            var mapId = request.MapId;
            var groupUid = request.GroupUid;
            
            var record = (await services.GetMapRecordsAsync(
                accountIds: [accountId],
                mapId: mapId,
                seasonId: groupUid))[0];
            
            return new PlayerScore
            {
                MedalType = record.Medals
            };
        }
    }

    /// <inheritdoc cref="IEndpoint" />
    public sealed class TestEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/totd/author-record", async (GetTOTDAuthorRecord query, PlayerMapInfo request) =>
            {
                return await query.HandleConsumeAsync(request);
            });
        }
    }

    protected override Consumer ConsumerComponent => new(services);

    public override async Task<TOTDInfo> HandleConsumeAsync(PlayerMapScore request)
    {
        return await base.HandleConsumeAsync(request);
    }
}