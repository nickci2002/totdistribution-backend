using ManiaAPI.NadeoAPI.Extensions.Hosting;
using NadeoCommunicator.Endpoints;
using TOTDistribution.Shared;

var builder = WebApplication.CreateBuilder(args);

// Json formatting for custom primitive types
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new MapGuidConverter());
    options.SerializerOptions.Converters.Add(new PlayerGuidConverter());
});

builder.Services.AddOpenApi();

// Inject NadeoCommunicator service with custom config + user agent
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("secrets.json")
    .AddEnvironmentVariables()
    .Build()
    .GetRequiredSection("NadeoAPI");
builder.Services.AddNadeoCommunicator(config);

builder.Services.AddEndpoints();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapEndpoints();

app.Run();