using System.Data;
using Dapper;
using Security.Application.Common.Interfaces;
using Security.Domain.Entities;
using Security.Domain.Exceptions;

namespace Security.Infrastructure.Repositories;

public class PerfilAccionRepository : IPerfilAccionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PerfilAccionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<PerfilAccion>> GetByPerfilAsync(int idPerfil, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT IdPerfil, IdAccion, IdCarga, UsuarioRegistro, FechaRegistro
            FROM seg.PerfilAccion
            WHERE IdPerfil = @IdPerfil;
            """;

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<PerfilAccion>(
                new CommandDefinition(sql, new { IdPerfil = idPerfil }, cancellationToken: cancellationToken));
            return result.AsList();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ocurrió un error al consultar las acciones del perfil.", ex);
        }
    }

    public async Task<bool> ExistsAsync(int idPerfil, int idAccion, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM seg.PerfilAccion WHERE IdPerfil = @IdPerfil AND IdAccion = @IdAccion;";

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, new { IdPerfil = idPerfil, IdAccion = idAccion }, cancellationToken: cancellationToken));
            return count > 0;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ocurrió un error al validar la asignación de la acción al perfil.", ex);
        }
    }

    public async Task AddAsync(PerfilAccion entity, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO seg.PerfilAccion (IdPerfil, IdAccion, IdCarga, UsuarioRegistro, FechaRegistro)
            VALUES (@IdPerfil, @IdAccion, @IdCarga, @UsuarioRegistro, @FechaRegistro);
            """;

        await connection.ExecuteAsync(new CommandDefinition(sql, entity, transaction, cancellationToken: cancellationToken));
    }

    public async Task RemoveAsync(int idPerfil, int idAccion, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM seg.PerfilAccion WHERE IdPerfil = @IdPerfil AND IdAccion = @IdAccion;";

        await connection.ExecuteAsync(new CommandDefinition(
            sql, new { IdPerfil = idPerfil, IdAccion = idAccion }, transaction, cancellationToken: cancellationToken));
    }

    public async Task RemoveAllForPerfilAsync(int idPerfil, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM seg.PerfilAccion WHERE IdPerfil = @IdPerfil;";

        await connection.ExecuteAsync(new CommandDefinition(sql, new { IdPerfil = idPerfil }, transaction, cancellationToken: cancellationToken));
    }
}
