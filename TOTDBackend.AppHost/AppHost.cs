using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var secrets = "../NadeoRefinery/secrets.json";
var config = new ConfigurationBuilder()
    .AddJsonFile(secrets, optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

//builder.AddContainer()
var redisConnString = config.GetSection("Redis").GetValue<string>("CM_ConnectionString")!;

builder.Configuration.AddJsonFile(secrets, optional: false, reloadOnChange: true);
builder.Build().Run();


/// <summary>
/// Resource representing pr
/// </summary>
/// <param name="name"></param>
/// <param name="containerNameOrId"></param>
internal sealed class ExternalContainerResource(string name, string containerNameOrId) : Resource(name)
{
    public string ContainerNameOrId { get; } = containerNameOrId;
}
