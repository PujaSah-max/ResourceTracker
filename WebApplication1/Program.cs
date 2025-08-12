using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Resource.DAO;
using Resource.DAO.Interface;
using Resource.Orchestration;
using Resource.Orchestration.Orchestration;
using WebApplication1.Controllers;
using WebApplication1.DAO;
using WebApplication1.DAO.Interface;
using WebApplication1.orchestration;
using WebApplication1.orchestration.Orches_Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;


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

// Register DAO and Orchestration For Authentication and Authorization
builder.Services.AddScoped<IAuthDAO, AuthDAO>();
builder.Services.AddScoped<IAuthOrchestration, AuthOrchestration>();

// Register DAO and Orchestration
builder.Services.AddScoped<IResourceDAO, ResourceDAO>();
builder.Services.AddScoped<IResourceOrchestration, ResourceOrchestration>();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

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
app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();
app.Run();
