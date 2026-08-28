using Microsoft.Extensions.DependencyInjection;
using Security.Application.Common.Interfaces;
using Security.Infrastructure.Persistence;
using Security.Infrastructure.Repositories;

namespace Security.Infrastructure;

/// <summary>Registers all Infrastructure-layer services: connection factory, unit of work and Dapper repositories.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPerfilRepository, PerfilRepository>();
        services.AddScoped<IOpcionRepository, OpcionRepository>();
        services.AddScoped<IAccionRepository, AccionRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPerfilOpcionRepository, PerfilOpcionRepository>();
        services.AddScoped<IPerfilAccionRepository, PerfilAccionRepository>();
        services.AddScoped<IUsuarioOpcionRepository, UsuarioOpcionRepository>();
        services.AddScoped<IUsuarioAccionRepository, UsuarioAccionRepository>();

        return services;
    }
}
