// using Hangfire;
// using Serilog;
// using TOTDistribution.NadeoRefinery.Common.Features;
// using TOTDistribution.NadeoRefinery.Features.Queries;

// namespace TOTDistribution.NadeoRefinery.Features.Jobs;

// public class TestJob : IJobSlice
// {
//     /// <summary>
//     /// Contains the job's actions
//     /// </summary>
//     internal class Job : IJob
//     {
//         private readonly ObtainCurrentTOTDInfo _currentTOTDInfo;

//         internal Job(ObtainCurrentTOTDInfo currentTOTDInfo)
//         {
//             _currentTOTDInfo = currentTOTDInfo;
//         }

//         public async Task Execute(IJobExecutionContext context)
//         {
//             var result = await _currentTOTDInfo.HandleAsync();
//             Log.Information(result.ToString() ?? "Something went wrong!");
//         }
//     }
    
//     private readonly IScheduler _scheduler;

//     internal TestJob(IScheduler scheduler)
//     {
//         _scheduler = scheduler;
//     }

//     public async Task ScheduleJob()
//     {
//         var job = JobBuilder.Create<Job>()
//             .WithIdentity("myJob", "group1")
//             .Build();
        
//         var trigger = TriggerBuilder.Create()
//             .WithIdentity("myTrigger", "group1")
//             .StartNow()
//             .WithSimpleSchedule(x => x
//                 .WithIntervalInMinutes(1)
//                 .RepeatForever())
//             .Build();
 
//         await _scheduler.ScheduleJob(job, trigger);
//     }
// }
