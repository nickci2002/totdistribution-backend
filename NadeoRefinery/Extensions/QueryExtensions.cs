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
}