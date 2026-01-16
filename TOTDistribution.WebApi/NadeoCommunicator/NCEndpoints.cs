using System.Reflection;
using ManiaAPI.NadeoAPI;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Quartz.Impl.Calendar;
using TOTDistribution.Shared.Endpoints;

namespace NadeoCommunicator.Endpoints;

// Helper files
public static class NadeoCommunicatorEndpoints
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var endpointTypes = Assembly
            .GetExecutingAssembly()
            .DefinedTypes
            .Where(type => type is { IsInterface: false, IsAbstract: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

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