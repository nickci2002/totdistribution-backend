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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/totd", async(ObtainCurrentTOTDInfo query) =>
{
    TOTDInfo totdInfo = await query.ExecuteQuery();
    return totdInfo;
});

app.MapGet("/placements", async() => "TOTDistribution.NadeoRefinery is running.");

app.Run();
