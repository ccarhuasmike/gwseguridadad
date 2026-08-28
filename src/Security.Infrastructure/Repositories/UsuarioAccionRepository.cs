using System.Data;
using Dapper;
using Security.Application.Common.Interfaces;
using Security.Domain.Entities;
using Security.Domain.Exceptions;

namespace Security.Infrastructure.Repositories;

public class UsuarioAccionRepository : IUsuarioAccionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UsuarioAccionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<UsuarioAccion>> GetByUsuarioAsync(int idUsuario, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT IdAccion, IdUsuario, IdCarga, UsuarioRegistro, FechaRegistro
            FROM seg.UsuarioAccion
            WHERE IdUsuario = @IdUsuario;
            """;

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<UsuarioAccion>(
                new CommandDefinition(sql, new { IdUsuario = idUsuario }, cancellationToken: cancellationToken));
            return result.AsList();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ocurrió un error al consultar las acciones del usuario.", ex);
        }
    }

    public async Task<bool> ExistsAsync(int idUsuario, int idAccion, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM seg.UsuarioAccion WHERE IdUsuario = @IdUsuario AND IdAccion = @IdAccion;";

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, new { IdUsuario = idUsuario, IdAccion = idAccion }, cancellationToken: cancellationToken));
            return count > 0;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ocurrió un error al validar la asignación de la acción al usuario.", ex);
        }
    }

    public async Task AddAsync(UsuarioAccion entity, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO seg.UsuarioAccion (IdAccion, IdUsuario, IdCarga, UsuarioRegistro, FechaRegistro)
            VALUES (@IdAccion, @IdUsuario, @IdCarga, @UsuarioRegistro, @FechaRegistro);
            """;

        await connection.ExecuteAsync(new CommandDefinition(sql, entity, transaction, cancellationToken: cancellationToken));
    }

    public async Task RemoveAsync(int idUsuario, int idAccion, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM seg.UsuarioAccion WHERE IdUsuario = @IdUsuario AND IdAccion = @IdAccion;";

        await connection.ExecuteAsync(new CommandDefinition(
            sql, new { IdUsuario = idUsuario, IdAccion = idAccion }, transaction, cancellationToken: cancellationToken));
    }

    public async Task RemoveAllForUsuarioAsync(int idUsuario, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM seg.UsuarioAccion WHERE IdUsuario = @IdUsuario;";

        await connection.ExecuteAsync(new CommandDefinition(sql, new { IdUsuario = idUsuario }, transaction, cancellationToken: cancellationToken));
    }
}
