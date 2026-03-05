using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using TOTDBackend.NadeoRefinery.Common.Extensions;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Features.Nadeo;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class NadeoSliceExtensions
{
    public static IServiceCollection AddNadeoSliceServices(
        this IServiceCollection services, IEnumerable<TypeInfo> types)
    {
        // var endpointTypes = types.GetServiceSlicesAsArray(typeof(NadeoSlice<>));
        // services.TryAddEnumerable(endpointTypes);

        // types.ToList().ForEach(t => Console.WriteLine("{0}", t));
        // Console.WriteLine("{0}", endpointTypes.Length);

        services.AddScoped<GetTOTDAuthorRecord>();
        services.AddScoped<GetTOTDDistribution>();
        services.AddScoped<GetTOTDInfo>();

        return services;
    }
}