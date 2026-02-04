using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using TOTDistribution.NadeoRefinery.Common.Endpoints;

namespace TOTDistribution.NadeoRefinery.Extensions;

public static class NadeoRefineryTestEndpoints
{
    public static IServiceCollection AddTestingEndpoints(this IServiceCollection services)
    {
        var endpointTypes = Assembly
            .GetExecutingAssembly()
            .DefinedTypes
            .Where(type => type is { IsInterface: false, IsAbstract: false } &&
                           type.IsAssignableTo(typeof(ITestableEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(ITestableEndpoint), type))
            .ToArray();

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