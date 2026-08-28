using Microsoft.OpenApi.Models;
using Security.Api.Common;
using Security.Api.Middleware;
using Security.Application;
using Security.Application.Common.Interfaces;
using Security.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Application (MediatR + FluentValidation) and Infrastructure (Dapper repositories + SQL Server) layers.
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// Authentication/authorization scaffolding: no scheme is configured yet, but the
// pipeline below is ready so a real mechanism (JWT, cookies, etc.) can be added
// later without touching Controllers or Application code.
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Security API - Gestión de Perfiles, Usuarios, Opciones y Acciones",
        Version = "v1",
        Description = "API de seguridad basada en CQRS + Dapper para administrar perfiles, opciones, acciones y sus permisos."
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Centralized exception handling: no Controller needs its own try/catch block.
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>Entry point partial class, exposed for WebApplicationFactory-based integration tests.</summary>
public partial class Program
{
}
