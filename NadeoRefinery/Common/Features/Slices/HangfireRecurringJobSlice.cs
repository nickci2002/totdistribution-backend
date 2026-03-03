using Hangfire;
using TOTDBackend.NadeoRefinery.Hangfire;

namespace TOTDBackend.NadeoRefinery.Common.Features;

internal interface IRecurringJobSlice
{
    void AddOrUpdate();
    Task HandleAsync();
}

/// <inheritdoc cref="IRecurringJobSlice">
internal abstract class RecurringJobSlice(RecurringJobManager jobManager) : IRecurringJobSlice
{
    protected RecurringJobProperties RecurringJobProperties { get; }
    
    protected string Id => RecurringJobProperties.Id;
    protected string CronExpression => RecurringJobProperties.CronExpression;
    protected TimeZoneInfo TimeZoneInfo => RecurringJobProperties.TimeZoneInfo;
    
    protected RecurringJobOptions JobOptions => new()
    {
        TimeZone = TimeZoneInfo
    };

    public void AddOrUpdate()
    {
        jobManager.AddOrUpdate(
            recurringJobId: Id,
            methodCall: () => HandleAsync(),
            cronExpression: CronExpression,
            options: JobOptions);
    }

    public abstract Task HandleAsync();
}