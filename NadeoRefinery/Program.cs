using System.Reflection;
using Serilog;
using TOTDBackend.NadeoRefinery.Extensions;
using TOTDBackend.Shared.JsonConverters;
using TOTDBackend.Shared.RabbitMQ;

#if WEB_API
var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .AddJsonFile("secrets.json")
    .AddEnvironmentVariables()
    .Build();

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new CustomPrimitiveConverterFactory()));

var sliceTypes = Assembly
    .GetExecutingAssembly()
    .DefinedTypes
    .Where((type) => type is { IsInterface: false, IsAbstract: false });

builder.Services.AddRedisDb(config.GetSection("Redis"));
builder.Services.AddNadeoAPIServices(config.GetSection("NadeoAPI"));
builder.Services.AddNadeoQuerySlices();
builder.Services.AddTestingEndpoints(sliceTypes);

builder.Host.AddSerilog();

builder.Services.AddHostedService(sp => 
{
    var logger = sp.GetRequiredService<ILogger<RabbitMqProducerBase>>();
    return new RabbitMqProducerBase(logger, QueueNames.RefineryBackend);
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.MapTestingEndpoints();

app.Run();

#elif WORKER

var builder = Host.CreateApplicationBuilder();

#else

#endif