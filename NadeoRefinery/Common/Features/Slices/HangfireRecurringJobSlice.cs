using System.Linq.Expressions;
using System.Reflection;
using Hangfire;
using TOTDBackend.NadeoRefinery.Common.Hangfire;

namespace TOTDBackend.NadeoRefinery.Common.Features;

public interface IRecurringJobSlice
{
    void AddOrUpdate(RecurringJobManager manager);
    Task HandleAsync();
}

/// <inheritdoc cref="IRecurringJobSlice">
internal abstract class RecurringJobSlice<TSelf> : IRecurringJobSlice
    where TSelf: IRecurringJobSlice
{
    protected abstract RecurringJobProperties JobProperties { get; }
    
    protected string Id => JobProperties.Id;
    protected string CronExpression => JobProperties.CronExpression;
    protected TimeZoneInfo TimeZoneInfo => JobProperties.TimeZoneInfo;
    
    protected RecurringJobOptions JobOptions => new() {
        TimeZone = TimeZoneInfo
    };

    // Reflection method for service registering
    public virtual void AddOrUpdate(RecurringJobManager manager)
    {
        manager.AddOrUpdate<TSelf>(
            Id,
            job => job.HandleAsync(),
            () => CronExpression,
            JobOptions);

    }

    public abstract Task HandleAsync();
}