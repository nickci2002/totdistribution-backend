using Serilog;
using TOTDBackend.NadeoRefinery;
using TOTDBackend.NadeoRefinery.Extensions;

#if WEB_API
var builder = WebApplication.CreateBuilder(args);

builder.Host.AddHost();

builder.Services.AddNadeoRefinery();

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
app.UseSerilogRequestLogging();

// Endpoint Test
app.MapTestingEndpoints();

app.Run();

#elif WORKER

var builder = Host.CreateApplicationBuilder();

#else

#endif