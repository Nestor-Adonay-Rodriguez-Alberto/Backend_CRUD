using Backend_CRUD.Application.Services;
using Backend_CRUD.CrossCutting.Configuration;
using Backend_CRUD.Domain.Interfaces.Repositories;
using Backend_CRUD.Domain.Interfaces.Services;
using Backend_CRUD.Infrastructure.Database;
using Backend_CRUD.Infrastructure.Persistence.Repositories;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Unex.Modulo9.Financiamientos.Application.Mapping;
using Unex.Modulo9.Financiamientos.CrossCutting.Helpers.CustomMapper;
using Unex.Modulo9.Financiamientos.CrossCutting.Helpers.CustomMapper.Interfaces;

var builder = FunctionsApplication.CreateBuilder(args);

// CONFIGURACIÓN JWT:
builder.Services.Configure<JwtSettings>(options =>
{
    options.SecretKey = Environment.GetEnvironmentVariable("JWT_SecretKey") ?? "MiClaveSecretaSuperSeguraParaJWT2024Backend";
    options.Issuer = Environment.GetEnvironmentVariable("JWT_Issuer") ?? "Backend_CRUD_API";
    options.Audience = Environment.GetEnvironmentVariable("JWT_Audience") ?? "Backend_CRUD_Client";
    options.ExpirationMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_ExpirationMinutes"), out var exp) ? exp : 60;
});

// INYECCIONES DE REPOSITORIOS:
builder.Services.AddScoped<ISeriesRepository, SeriesRepository>();
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();

// INYECCIONES DE SERVICES:
builder.Services.AddScoped<ISeriesService, SeriesService>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();




// Configurar la aplicaci�n de Azure Functions
builder.ConfigureFunctionsWebApplication();

// Configurar serializaci�n en camelCase
builder.Services.AddSingleton<JsonSerializerSettings>(new JsonSerializerSettings
{
    ContractResolver = new CamelCasePropertyNamesContractResolver(),
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    NullValueHandling = NullValueHandling.Ignore,
    Formatting = Formatting.Indented
});


// Configurar AutoMapper personalizado
builder.Services.AddSingleton<ICustomMapper>(provider =>
{
    var mapper = new AutoMapperHelper();
    CustomMapperConfigurations.RegisterMappings(mapper);
    return mapper;
});


// Configurar DbContext con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("SQLConnectionString");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("La cadena de conexión 'SQLConectionString' no está configurada.");
    }

    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.CommandTimeout(300));
});

builder.Build().Run();
