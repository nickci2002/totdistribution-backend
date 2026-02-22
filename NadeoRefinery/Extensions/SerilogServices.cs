using Serilog;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class SerilogServices
{
    public static IHostBuilder AddSerilog(this IHostBuilder host)
    {
        host.UseSerilog((context, configuration) => 
            configuration.ReadFrom.Configuration(context.Configuration));

        return host;
    }
}