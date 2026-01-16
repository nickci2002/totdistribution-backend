namespace TOTDistribution.Shared.Endpoints;

public interface IEndpoint
{
    abstract void MapEndpoint(IEndpointRouteBuilder app);
}