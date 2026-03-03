namespace TOTDBackend.NadeoRefinery.Extensions;

public static class SerilogServices
{
    /// <summary>
    /// Used for Web project
    /// </summary>
    public static IHostBuilder AddSerilog(this IHostBuilder host) =>
        host.UseSerilog((context, configuration) => 
            configuration.ReadFrom.Configuration(context.Configuration));
    
    /// <summary>
    /// Used for Worker project
    /// </summary>
    public static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSerilog(config =>
            config.ReadFrom.Configuration(configuration)); 
}