using Hangfire;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Features.Nadeo;
using TOTDBackend.NadeoRefinery.Common.Hangfire;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace TOTDBackend.NadeoRefinery.Features.Jobs;

internal sealed class BeginTOTDJob(IServiceProvider provider) : RecurringJobSlice<BeginTOTDJob>
{
    public sealed class TestEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/jobs/begin-totd", async([FromServices] BeginTOTDJob job) =>
            {
                await job.HandleAsync();
            });
        }
    }

    protected override RecurringJobProperties JobProperties => new()
    {
        Id = "begin-totd",
        CronExpression = "01 19 * * *", // One minute after the TOTD switches
        TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time")
    };

    public override async Task HandleAsync()
    {
        Log.Information("""Executing job "StartTOTDFetchJob\" ...""");
        Log.Verbose("Id: {id} | Cron: {cron} | Timezone: {tz}", Id,  CronExpression,  TimeZoneInfo);
        
        try
        {
            Log.Debug("Creating async scope and retrieving GetTOTDInfo slice...");

            await using var scope = provider.CreateAsyncScope();
            var totdInfoService = scope.ServiceProvider.GetService<GetTOTDInfo>();

            if (totdInfoService is null)
            {
                Log.Error("""Failed to retrieve GetTOTDInfo slice in "StartTOTDFetchJob"!""");
                throw new Exception();
            }

            var totdInfo = await totdInfoService.HandleConsumeAndStorageAsync();
        }
        catch
        {
            Log.Error("""There was an error occurred executing job "StartTOTDFetchJob".""");
            return;
        }
        finally
        {
            Log.Information("""Finished executing job "StartTOTDFetchJob"!""");
        }
    }
}