using System.Data;
using Dapper;
using Security.Application.Common.Interfaces;
using Security.Domain.Entities;
using Security.Domain.Exceptions;

namespace Security.Infrastructure.Repositories;

public class UsuarioOpcionRepository : IUsuarioOpcionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UsuarioOpcionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<UsuarioOpcion>> GetByUsuarioAsync(int idUsuario, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT IdUsuario, IdOpcion, IdCarga, UsuarioRegistro, FechaRegistro
            FROM seg.UsuarioOpcion
            WHERE IdUsuario = @IdUsuario;
            """;

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<UsuarioOpcion>(
                new CommandDefinition(sql, new { IdUsuario = idUsuario }, cancellationToken: cancellationToken));
            return result.AsList();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ocurrió un error al consultar las opciones del usuario.", ex);
        }
    }

    public async Task<bool> ExistsAsync(int idUsuario, int idOpcion, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM seg.UsuarioOpcion WHERE IdUsuario = @IdUsuario AND IdOpcion = @IdOpcion;";

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, new { IdUsuario = idUsuario, IdOpcion = idOpcion }, cancellationToken: cancellationToken));
            return count > 0;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ocurrió un error al validar la asignación de la opción al usuario.", ex);
        }
    }

    public async Task AddAsync(UsuarioOpcion entity, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO seg.UsuarioOpcion (IdUsuario, IdOpcion, IdCarga, UsuarioRegistro, FechaRegistro)
            VALUES (@IdUsuario, @IdOpcion, @IdCarga, @UsuarioRegistro, @FechaRegistro);
            """;

        await connection.ExecuteAsync(new CommandDefinition(sql, entity, transaction, cancellationToken: cancellationToken));
    }

    public async Task RemoveAsync(int idUsuario, int idOpcion, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM seg.UsuarioOpcion WHERE IdUsuario = @IdUsuario AND IdOpcion = @IdOpcion;";

        await connection.ExecuteAsync(new CommandDefinition(
            sql, new { IdUsuario = idUsuario, IdOpcion = idOpcion }, transaction, cancellationToken: cancellationToken));
    }

    public async Task RemoveAllForUsuarioAsync(int idUsuario, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM seg.UsuarioOpcion WHERE IdUsuario = @IdUsuario;";

        await connection.ExecuteAsync(new CommandDefinition(sql, new { IdUsuario = idUsuario }, transaction, cancellationToken: cancellationToken));
    }
}
