using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using TOTDistribution.NadeoRefinery.Common.Extensions;
using TOTDistribution.NadeoRefinery.Common.Features;

namespace TOTDistribution.NadeoRefinery.Extensions;

public static class QueryExtensions
{
    public static IServiceCollection AddNadeoQuerySlices(
        this IServiceCollection services, IEnumerable<TypeInfo> types)
    {
        var endpointTypes = types.GetServiceSlicesAsArray(typeof(INadeoQuerySlice));
        services.TryAddEnumerable(endpointTypes);

        return services;
    }
}