using System.Runtime.InteropServices;
using ManiaAPI.NadeoAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Models.Requests;
using TOTDBackend.NadeoRefinery.Models.Responses;

namespace TOTDBackend.NadeoRefinery.Features.Nadeo;

/// <inheritdoc cref="NadeoCommunicatorSlice{TResp}" />
internal sealed class GetTOTDAuthorRecord(NadeoServices nadeo)
    : NadeoCommunicatorSlice<PlayerMapInfo, PlayerScore>
{
    /// <inheritdoc cref="INadeoConsumerComponent{TReq, TResp}" />
    internal class Consumer(NadeoServices nadeo)
        : INadeoConsumerComponent<PlayerMapInfo, PlayerScore>
    {
        public async Task<PlayerScore> FetchDataAsync(PlayerMapInfo request)
        {
            var accountId = request.AccountId;
            var mapId = request.MapId;
            var groupUid = request.GroupUid;
            
            var record = (await nadeo.GetMapRecordsAsync(
                accountIds: [accountId],
                mapId: mapId,
                seasonId: groupUid))[0];
            
            return new PlayerScore
            {
                MedalCount = record.Medals
            };
        }
    }

    /// <inheritdoc cref="IEndpoint" />
    public sealed class TestEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/nadeo/author-record", async (
                [FromServices] GetTOTDAuthorRecord query, [FromBody] PlayerMapInfo request) =>
                    await query.HandleConsumeAsync(request));
        }
    }

    protected override Consumer ConsumerComponent => new(nadeo);

    public override async Task<PlayerScore> HandleConsumeAsync(PlayerMapInfo request)
    {
        return await base.HandleConsumeAsync(request);
    }
}