using Hangfire;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Common.Hangfire;
using TOTDBackend.NadeoRefinery.Common.Utils;
using TOTDBackend.NadeoRefinery.Features.Nadeo;
using TOTDBackend.NadeoRefinery.Models.Mappings;

namespace TOTDBackend.NadeoRefinery.Features.Jobs;

internal sealed class EndTOTDJob(IServiceProvider provider) : RecurringJobSlice<EndTOTDJob>
{
    public sealed class TestEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/jobs/end-totd", async([FromServices] EndTOTDJob job) =>
            {
                await job.HandleAsync();
            });
        }
    }

    protected override RecurringJobProperties JobProperties => new() {
        Id = "end-totd",
        CronExpression = "51 18 * * *", // One minute before the TOTD switches
        TimeZoneInfo = ParisianTimeHelper.ParisianTimeZoneInfo
    };

    public override async Task HandleAsync()
    {
        Log.Information("""Executing job "EndTOTDFetchJob\" ...""");
        Log.Verbose("Id: {id} | Cron: {cron} | Timezone: {tz}", Id, CronExpression, TimeZoneInfo);

        try
        {
            Log.Debug("Creating async scope and retrieving slices...");

            await using var scope = provider.CreateAsyncScope();
            var totdDistributionService = scope.ServiceProvider.GetService<GetTOTDDistribution>();
            var totdInfoService = scope.ServiceProvider.GetService<GetTOTDInfo>();

            if (totdInfoService is null)
            {
                Log.Error("""Failed to retrieve GetTOTDInfo slice in "EndTOTDFetchJob"!""");
                throw new Exception();
            }

            if (totdDistributionService is null)
            {
                Log.Error("""Failed to retrieve GetTOTDDistribution slice in "EndTOTDFetchJob"!""");
                throw new Exception();
            }

            Log.Debug("""Retrieving current totd information from "GetTOTDInfo"...""");

            var key = TOTDDayFinder.CreateRedisTOTDIdKeyAsString();
            var totdInfo = totdInfoService.HandleRetrieval(key);

            Log.Verbose("Key: {key} | TOTD Info: {info}", key, JsonSerializer.Serialize(totdInfo));

            var mapMedals = totdInfo.ToMapMedals();
            var distribution = await totdDistributionService.HandleConsumeAsync(mapMedals);

            // Just store data in redis for now... we don't have the backend set up yet...
            // In the future, we'll just set up another thing for updates
            await totdDistributionService.HandleStorageAsync(distribution);
        }
        catch
        {
            Log.Error("""There was an error occurred executing job "EndTOTDFetchJob".""");
            return;
        }
        finally
        {
            Log.Information("""Finished executing job "EndTOTDFetchJob"!""");
        }
    }
}