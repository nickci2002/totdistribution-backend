#if WEB_API
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using TOTDBackend.NadeoRefinery.Common.Extensions;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class NadeoRefineryTestEndpoints
{
    public static IServiceCollection AddTestingEndpoints(
        this IServiceCollection services, IEnumerable<TypeInfo> types)
    {
        var endpointTypes = types.GetServiceSlicesAsArray(typeof(ITestableEndpoint));
        services.TryAddEnumerable(endpointTypes);

        return services;
    }
    
    public static IApplicationBuilder MapTestingEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<ITestableEndpoint>>();
        foreach (var endpoint in endpoints)
        {
            endpoint.MapTestingEndpoint(app);
        }

        return app;
    }
}
#endif