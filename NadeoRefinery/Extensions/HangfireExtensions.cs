using System.Reflection;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Features.Jobs;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class HangfireExtensions
{
    public static IServiceCollection AddHangfireServices(
        this IServiceCollection services, IConnectionMultiplexer multiplexer)
    {
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseRedisStorage(multiplexer, new RedisStorageOptions()));
        services.AddHangfireServer();

        return services;
    }

    public static IServiceCollection AddJobSliceServices(
        this IServiceCollection services, IEnumerable<TypeInfo> types)
    {
        services.AddSingleton<RecurringJobManager>();

        services.AddScoped<IRecurringJobSlice, BeginTOTDJob>();
        services.AddScoped<IRecurringJobSlice, EndTOTDJob>();
        
        // var sliceServices = types
        //     .Where(t => typeof(IRecurringJobSlice).IsAssignableFrom(t.AsType()))
        //     .Select(t => ServiceDescriptor.Scoped(typeof(IRecurringJobSlice), t))
        //     .ToArray();

        // services.TryAddEnumerable(sliceServices);

        return services;
    }

    public static IHost AddOrUpdateSlices(this IHost app)
    {
        var manager = app.Services.GetRequiredService<RecurringJobManager>();
        var slices = app.Services.GetRequiredService<IEnumerable<IRecurringJobSlice>>();
        foreach (var slice in slices)
        {
            slice.AddOrUpdate(manager);
        }

        return app;
    }
}