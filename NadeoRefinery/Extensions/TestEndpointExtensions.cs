using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using TOTDBackend.NadeoRefinery.Common.Extensions;

namespace TOTDBackend.NadeoRefinery.Extensions;

#if WEB_API
public static class TestEndpointExtensions
{
    public static IServiceCollection AddTestingEndpoints(
        this IServiceCollection services, IEnumerable<TypeInfo> types)
    {
        var endpointTypes = types.GetServiceSlicesAsArray(typeof(IEndpoint));
        services.TryAddEnumerable(endpointTypes);

        return services;
    }
    
    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();
        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}
#endif