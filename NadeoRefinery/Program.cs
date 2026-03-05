using System.Reflection;
using Serilog.Extensions.Hosting;
using TOTDBackend.NadeoRefinery.Extensions;
using TOTDBackend.NadeoRefinery.Features.Nadeo;
using TOTDBackend.Shared.RabbitMQ;

#if WEB_API
// If implemented as a web project (for testing Nadeo API)
var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .AddJsonFile("secrets.json")
    .AddEnvironmentVariables()
    .Build();

builder.Host.AddSerilogHost();
builder.Services.ConfigureJsonProperties();

var sliceTypes = Assembly
    .GetExecutingAssembly()
    .DefinedTypes
    .Where((type) => type is { IsInterface: false, IsAbstract: false });

builder.Services.AddNadeoAPIServices(config.GetSection("NadeoAPI"));
builder.Services.AddNadeoSliceServices(sliceTypes);
builder.Services.AddRedisDbServices(config.GetSection("Redis"));

builder.Services.AddTestingEndpoints(sliceTypes);

// builder.Services.AddHostedService(sp => 
// {
//     var logger = sp.GetRequiredService<ILogger<RabbitMQPublisherBase<int>>>();
//     return new RabbitMQPublisherBase(logger, QueueNames.RefineryBackend);
// });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.MapEndpoints();

app.Run();

#elif WORKER
// If implemented as a worker service (for production)
var builder = Host.CreateApplicationBuilder();

//GlobalConfiguration.Configuration

var config = new ConfigurationBuilder()
    .AddJsonFile("secrets.json")
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddSerilogServices(builder.Configuration);
builder.Services.ConfigureJsonProperties();

var sliceTypes = Assembly
    .GetExecutingAssembly()
    .DefinedTypes
    .Where((type) => type is { IsInterface: false, IsAbstract: false });

builder.Services.AddHangfireServices();
builder.Services.AddNadeoAPIServices(config.GetSection("NadeoAPI"));
builder.Services.AddNadeoSliceServices(sliceTypes);
builder.Services.AddRedisDbServices(config.GetSection("Redis"));

var app = builder.Build();

await app.RunAsync();

#endif