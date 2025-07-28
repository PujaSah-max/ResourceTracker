using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplication1.Controllers;
using WebApplication1.DAO;
using WebApplication1.DAO.Interface;
using WebApplication1.orchestration;
using WebApplication1.orchestration.Orches_Interface;

var builder = WebApplication.CreateBuilder(args);

// Read Swagger config from appsettings
var swaggerConfig = builder.Configuration.GetSection("Swagger");
var swaggerTitle = swaggerConfig["Title"] ?? "API";
var swaggerVersion = swaggerConfig["Version"] ?? "v1";
var swaggerDescription = swaggerConfig["Description"] ?? "";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

//  services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(swaggerVersion, new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = swaggerTitle,
        Version = swaggerVersion,
        Description = swaggerDescription
    });
});

// Register DAO and Orchestration
builder.Services.AddScoped<IResourceDAO, ResourceDAO>();
builder.Services.AddScoped<IResourceOrchestration, ResourceOrchestration>();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

//  Log4Net logger provider
var loggerFactory = app.Services.GetService<ILoggerFactory>();
loggerFactory.AddLog4Net("log4net.config");
app.UseCors("AllowAll");

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
