using Serilog;

namespace TOTDistribution.NadeoRefinery.Services;

public static class SerilogServices
{
    public static void AddSerilog(this IHostBuilder host)
    {
        host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));
    }
}