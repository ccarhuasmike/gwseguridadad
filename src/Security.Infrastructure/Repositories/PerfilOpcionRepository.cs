using System.Data;
using Dapper;
using Security.Application.Common.Interfaces;
using Security.Domain.Entities;
using Security.Domain.Exceptions;

namespace Security.Infrastructure.Repositories;

public class PerfilOpcionRepository : IPerfilOpcionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PerfilOpcionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<PerfilOpcion>> GetByPerfilAsync(int idPerfil, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT IdPerfil, IdOpcion, IdCarga, UsuarioRegistro, FechaRegistro
            FROM seg.PerfilOpcion
            WHERE IdPerfil = @IdPerfil;
            """;

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<PerfilOpcion>(
                new CommandDefinition(sql, new { IdPerfil = idPerfil }, cancellationToken: cancellationToken));
            return result.AsList();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ocurrió un error al consultar las opciones del perfil.", ex);
        }
    }

    public async Task<bool> ExistsAsync(int idPerfil, int idOpcion, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM seg.PerfilOpcion WHERE IdPerfil = @IdPerfil AND IdOpcion = @IdOpcion;";

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, new { IdPerfil = idPerfil, IdOpcion = idOpcion }, cancellationToken: cancellationToken));
            return count > 0;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ocurrió un error al validar la asignación de la opción al perfil.", ex);
        }
    }

    public async Task AddAsync(PerfilOpcion entity, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO seg.PerfilOpcion (IdPerfil, IdOpcion, IdCarga, UsuarioRegistro, FechaRegistro)
            VALUES (@IdPerfil, @IdOpcion, @IdCarga, @UsuarioRegistro, @FechaRegistro);
            """;

        await connection.ExecuteAsync(new CommandDefinition(sql, entity, transaction, cancellationToken: cancellationToken));
    }

    public async Task RemoveAsync(int idPerfil, int idOpcion, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM seg.PerfilOpcion WHERE IdPerfil = @IdPerfil AND IdOpcion = @IdOpcion;";

        await connection.ExecuteAsync(new CommandDefinition(
            sql, new { IdPerfil = idPerfil, IdOpcion = idOpcion }, transaction, cancellationToken: cancellationToken));
    }

    public async Task RemoveAllForPerfilAsync(int idPerfil, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM seg.PerfilOpcion WHERE IdPerfil = @IdPerfil;";

        await connection.ExecuteAsync(new CommandDefinition(sql, new { IdPerfil = idPerfil }, transaction, cancellationToken: cancellationToken));
    }
}
