using Hangfire;

namespace TOTDistribution.NadeoRefinery.Extensions;

public static class HangfireExtensions
{
    public static void AddHangfireServices(this IServiceCollection services)
    {
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings());

        services.AddHangfireServer();
    }
}