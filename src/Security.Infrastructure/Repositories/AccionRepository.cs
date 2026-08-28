using Dapper;
using Security.Application.Common.Interfaces;
using Security.Domain.Entities;
using Security.Domain.Exceptions;

namespace Security.Infrastructure.Repositories;

public class AccionRepository : IAccionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AccionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Accion?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, IdOpcion, Nombre, Descripcion, Activo, UsuarioRegistro, FechaRegistro, UsuarioModifica, FechaModifica
            FROM seg.Accion
            WHERE Id = @Id;
            """;

        return await ExecuteAsync(async connection =>
            await connection.QuerySingleOrDefaultAsync<Accion>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken)));
    }

    public async Task<IReadOnlyList<Accion>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT Id, IdOpcion, Nombre, Descripcion, Activo, UsuarioRegistro, FechaRegistro, UsuarioModifica, FechaModifica
            FROM seg.Accion
            {(includeInactive ? string.Empty : "WHERE Activo = 1")}
            ORDER BY Nombre;
            """;

        return await ExecuteAsync(async connection =>
            (await connection.QueryAsync<Accion>(new CommandDefinition(sql, cancellationToken: cancellationToken))).AsList());
    }

    public async Task<IReadOnlyList<Accion>> GetByOpcionAsync(int idOpcion, bool includeInactive, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT Id, IdOpcion, Nombre, Descripcion, Activo, UsuarioRegistro, FechaRegistro, UsuarioModifica, FechaModifica
            FROM seg.Accion
            WHERE IdOpcion = @IdOpcion {(includeInactive ? string.Empty : "AND Activo = 1")}
            ORDER BY Nombre;
            """;

        return await ExecuteAsync(async connection =>
            (await connection.QueryAsync<Accion>(new CommandDefinition(sql, new { IdOpcion = idOpcion }, cancellationToken: cancellationToken))).AsList());
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM seg.Accion WHERE Id = @Id;";

        return await ExecuteAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken)) > 0);
    }

    public async Task<bool> BelongsToOpcionAsync(int idAccion, int idOpcion, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM seg.Accion WHERE Id = @IdAccion AND IdOpcion = @IdOpcion;";

        return await ExecuteAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, new { IdAccion = idAccion, IdOpcion = idOpcion }, cancellationToken: cancellationToken)) > 0);
    }

    public async Task<int> CreateAsync(Accion accion, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO seg.Accion (IdOpcion, Nombre, Descripcion, Activo, UsuarioRegistro, FechaRegistro)
            OUTPUT INSERTED.Id
            VALUES (@IdOpcion, @Nombre, @Descripcion, @Activo, @UsuarioRegistro, @FechaRegistro);
            """;

        return await ExecuteAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, accion, cancellationToken: cancellationToken)));
    }

    public async Task UpdateAsync(Accion accion, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE seg.Accion
            SET Nombre = @Nombre,
                Descripcion = @Descripcion,
                Activo = @Activo,
                UsuarioModifica = @UsuarioModifica,
                FechaModifica = @FechaModifica
            WHERE Id = @Id;
            """;

        await ExecuteAsync(async connection =>
            await connection.ExecuteAsync(new CommandDefinition(sql, accion, cancellationToken: cancellationToken)));
    }

    public async Task DeactivateAsync(int id, int usuarioModifica, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE seg.Accion
            SET Activo = 0, UsuarioModifica = @UsuarioModifica, FechaModifica = @FechaModifica
            WHERE Id = @Id;
            """;

        await ExecuteAsync(async connection =>
            await connection.ExecuteAsync(new CommandDefinition(
                sql,
                new { Id = id, UsuarioModifica = usuarioModifica, FechaModifica = DateTime.UtcNow },
                cancellationToken: cancellationToken)));
    }

    private async Task<T> ExecuteAsync<T>(Func<System.Data.IDbConnection, Task<T>> operation)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            return await operation(connection);
        }
        catch (AppExceptionBase)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ocurrió un error al acceder a la información de acciones.", ex);
        }
    }
}
