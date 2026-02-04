using Serilog;
using TOTDistribution.NadeoRefinery;
using TOTDistribution.NadeoRefinery.Extensions;
using TOTDistribution.NadeoRefinery.Features.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNadeoRefinery();
builder.Services.AddTestingEndpoints();

builder.Services.AddTransient<ObtainCurrentTOTDInfo>();
builder.Services.AddTransient<GetTOTDDistribution>();

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

// Endpoint Test
app.MapTestingEndpoints();

app.Run();
