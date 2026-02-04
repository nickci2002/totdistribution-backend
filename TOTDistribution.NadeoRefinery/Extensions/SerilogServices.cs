using Serilog;

namespace TOTDistribution.NadeoRefinery.Extensions;

public static class SerilogServices
{
    public static void AddSerilog(this IHostBuilder host)
    {
        host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));
    }
}