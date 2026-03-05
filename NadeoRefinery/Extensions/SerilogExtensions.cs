namespace TOTDBackend.NadeoRefinery.Extensions;

public static class SerilogExtensions
{
    /// <summary>
    /// Used for Web project
    /// </summary>
    public static IHostBuilder AddSerilogHost(this IHostBuilder host) =>
        host.UseSerilog((context, configuration) => 
            configuration.ReadFrom.Configuration(context.Configuration));
    
    /// <summary>
    /// Used for Worker project
    /// </summary>
    public static IServiceCollection AddSerilogServices(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSerilog(config =>
            config.ReadFrom.Configuration(configuration)); 
}