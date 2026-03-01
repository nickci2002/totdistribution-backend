namespace TOTDBackend.NadeoRefinery.Hangfire;

internal readonly record struct RecurringJobProperties(
    string Id,
    string CronExpression,
    TimeZoneInfo TimeZoneInfo
);