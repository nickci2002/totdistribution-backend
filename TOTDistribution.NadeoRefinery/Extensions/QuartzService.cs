using Quartz;

namespace TOTDistribution.NadeoRefinery.Extensions;

public static class QuartzService
{
    public static void AddQuartzServices(this IServiceCollection services, IConfiguration config)
    {
        // var scheduler = CronScheduleBuilder.DailyAtHourAndMinute(19, 0).InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));
        // services.AddQuartz(qt =>
        // {
        //     qt.SchedulerId = "NadeoRefineryScheduler";
        //     qt.SchedulerName = "NadeoRefineryQuartzScheduler";

        //     qt.AddJob<ObtainCurrentTOTDInfoJob>(opts => 
        //     {
        //         opts.WithIdentity("ObtainCurrentTOTDInfoJob", "NadeoRefineryJobsGroup");
        //         opts.StoreDurably();
        //     });

        //     qt.AddTrigger(opts =>
        //     {
        //         opts.ForJob("ObtainCurrentTOTDInfoJob", "NadeoRefineryJobsGroup");
        //         opts.WithIdentity("ObtainCurrentTOTDInfoJobTrigger", "NadeoRefineryJobsGroup");
        //         opts.WithSchedule(scheduler);
        //     });
        // });

        // services.AddQuartzHostedService(qt => qt.WaitForJobsToComplete = true);
    }
}