using CrashAnalytics.Authentication;
using CrashAnalytics.Utils;
using CrashLogger.Services;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "The API Key to access the API",
        Type = SecuritySchemeType.ApiKey,
        Name = "ApiKey",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme",
    });
    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = ParameterLocation.Header
    };
    var requirement = new OpenApiSecurityRequirement
    {
        {scheme, new List<string>() }
    };
    c.AddSecurityRequirement(requirement);
});

var envReader = new EnvReader();
envReader.Build();

var configuration = builder.Configuration;
var databaseConnection = string.Format(configuration.GetConnectionString("DatabaseConnection"), new string[] {
    envReader.GetValue("DB_CONTAINER_NAME"),
    envReader.GetValue("DB_PORT"),
    envReader.GetValue("DB_NAME"),
    envReader.GetValue("DB_USER"),
    envReader.GetValue("DB_PASSWORD"),
});

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(databaseConnection));

builder.Services.AddScoped<ICacheService, CacheService>();

var app = builder.Build();

DatabaseServiceManagement.MigrationInitialization(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();
