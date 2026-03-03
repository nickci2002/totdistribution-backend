using Hangfire;
using TOTDBackend.NadeoRefinery.Common.Features;

namespace TOTDBackend.NadeoRefinery.Features.Jobs;

internal sealed class TestJob(RecurringJobManager jobManager) : RecurringJobSlice(jobManager)
{
    public override Task HandleAsync()
    {
        throw new NotImplementedException();
    }
}