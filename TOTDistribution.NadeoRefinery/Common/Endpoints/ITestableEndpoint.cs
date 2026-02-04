namespace TOTDistribution.NadeoRefinery.Common.Endpoints;

public interface ITestableEndpoint
{
    abstract void MapTestingEndpoint(IEndpointRouteBuilder app);
}