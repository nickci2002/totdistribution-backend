namespace TOTDBackend.NadeoRefinery.Common.Endpoints;

public interface IEndpoint
{
    abstract void MapEndpoint(IEndpointRouteBuilder app);
}

#if !WEB_API
// Empty implementation of Web project packages to satisfy the compiler
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