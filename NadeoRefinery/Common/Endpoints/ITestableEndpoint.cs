namespace TOTDBackend.NadeoRefinery.Common.Endpoints;

public interface ITestableEndpoint
{
    abstract void MapTestingEndpoint(IEndpointRouteBuilder app);
}

// Empty implementation of Web project packages to satisfy the compiler
#if !WEB_API
internal interface IEndpointRouteBuilder;

internal static class EmptyEndpointExtensions
{
    internal static void MapGet(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate requestDelegate)
    {    
    }

    internal static void MapPost(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        Delegate requestDelegate)
    {  
    }
}
#endif