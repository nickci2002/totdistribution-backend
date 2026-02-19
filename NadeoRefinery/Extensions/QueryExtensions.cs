using System.Reflection;
using TOTDBackend.NadeoRefinery.Features.Queries;

namespace TOTDBackend.NadeoRefinery.Extensions;

public static class QueryExtensions
{
    public static IServiceCollection AddNadeoQuerySlices(this IServiceCollection services)
    {
        services.AddTransient<GetTOTDDistribution>();
        services.AddTransient<ObtainCurrentTOTDInfo>();

        return services;
    }

    // public static IServiceCollection AddNadeoQuerySlices(
    //     this IServiceCollection services, IEnumerable<TypeInfo> types)
    // {
    //     var slices = AddSlices(types, typeof(INadeoQuerySlice<>));
    //     var slicesWithRequests = AddSlices(types, typeof(INadeoQuerySlice<,>));
    //     services.TryAddEnumerable(slices.Concat(slicesWithRequests));
        
    //     return services;
    // }

    // private static IEnumerable<ServiceDescriptor> AddSlices(
    //     IEnumerable<TypeInfo> types, Type typeToAssign)
    // {
    //     return types
    //         .Where(typeToAssign.IsAssignableFrom)
    //         .Select(type => ServiceDescriptor.Transient(type, type));
    // }
}