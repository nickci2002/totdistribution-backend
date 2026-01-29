using Redis.OM;
using Serilog;
using TOTDistribution.NadeoRefinery;
using TOTDistribution.NadeoRefinery.Entities;
using TOTDistribution.NadeoRefinery.Features.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNadeoRefinery();

// Register ObtainCurrentTOTDInfo for dependency injection
builder.Services.AddTransient<ObtainCurrentTOTDInfo>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Host.AddHost();
//onfigureHostBuilder

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.MapGet("/totd", async(ObtainCurrentTOTDInfo query, RedisConnectionProvider provider) =>
{
    TOTDInfo totdInfo = await query.ExecuteQuery();
    await query.StoreDataAsync(totdInfo);
    
    return totdInfo;
});

app.MapGet("/placements", async() => "TOTDistribution.NadeoRefinery is running.");

app.Run();
