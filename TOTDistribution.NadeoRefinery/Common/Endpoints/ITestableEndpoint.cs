namespace TOTDistribution.NadeoRefinery.Common.Endpoints;

public interface ITestableEndpoint
{
    abstract void MapTestingEndpoint(IEndpointRouteBuilder app);
}

// Empty implementation of Web project packages to satisfy the compiler
#if WORKER
public interface IEndpointRouteBuilder;

public static class EmptyEndpointExtensions
{
    public static void MapGet(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate requestDelegate)
    {    
    }

    public static void MapPost(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate requestDelegate)
    {  
    }
}
#endif