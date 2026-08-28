using Dapper;
using Security.Application.Common.Interfaces;
using Security.Domain.Entities;
using Security.Domain.Exceptions;

namespace Security.Infrastructure.Repositories;

public class OpcionRepository : IOpcionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OpcionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Opcion?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, IdPadre, Nombre, Descripcion, Ruta, Orden, Visible, Activo,
                   UsuarioRegistro, FechaRegistro, UsuarioModifica, FechaModifica
            FROM seg.Opcion
            WHERE Id = @Id;
            """;

        return await ExecuteAsync(async connection =>
            await connection.QuerySingleOrDefaultAsync<Opcion>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken)));
    }

    public async Task<IReadOnlyList<Opcion>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT Id, IdPadre, Nombre, Descripcion, Ruta, Orden, Visible, Activo,
                   UsuarioRegistro, FechaRegistro, UsuarioModifica, FechaModifica
            FROM seg.Opcion
            {(includeInactive ? string.Empty : "WHERE Activo = 1")}
            ORDER BY Orden, Nombre;
            """;

        return await ExecuteAsync(async connection =>
            (await connection.QueryAsync<Opcion>(new CommandDefinition(sql, cancellationToken: cancellationToken))).AsList());
    }

    public async Task<IReadOnlyList<Opcion>> GetChildrenAsync(int idPadre, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, IdPadre, Nombre, Descripcion, Ruta, Orden, Visible, Activo,
                   UsuarioRegistro, FechaRegistro, UsuarioModifica, FechaModifica
            FROM seg.Opcion
            WHERE IdPadre = @IdPadre AND Activo = 1
            ORDER BY Orden, Nombre;
            """;

        return await ExecuteAsync(async connection =>
            (await connection.QueryAsync<Opcion>(new CommandDefinition(sql, new { IdPadre = idPadre }, cancellationToken: cancellationToken))).AsList());
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM seg.Opcion WHERE Id = @Id;";

        return await ExecuteAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken)) > 0);
    }

    public async Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM seg.Opcion WHERE IdPadre = @Id AND Activo = 1;";

        return await ExecuteAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken)) > 0);
    }

    public async Task<bool> HasAccionesAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM seg.Accion WHERE IdOpcion = @Id AND Activo = 1;";

        return await ExecuteAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken)) > 0);
    }

    public async Task<int> CreateAsync(Opcion opcion, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO seg.Opcion (IdPadre, Nombre, Descripcion, Ruta, Orden, Visible, Activo, UsuarioRegistro, FechaRegistro)
            OUTPUT INSERTED.Id
            VALUES (@IdPadre, @Nombre, @Descripcion, @Ruta, @Orden, @Visible, @Activo, @UsuarioRegistro, @FechaRegistro);
            """;

        return await ExecuteAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, opcion, cancellationToken: cancellationToken)));
    }

    public async Task UpdateAsync(Opcion opcion, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE seg.Opcion
            SET IdPadre = @IdPadre,
                Nombre = @Nombre,
                Descripcion = @Descripcion,
                Ruta = @Ruta,
                Orden = @Orden,
                Visible = @Visible,
                Activo = @Activo,
                UsuarioModifica = @UsuarioModifica,
                FechaModifica = @FechaModifica
            WHERE Id = @Id;
            """;

        await ExecuteAsync(async connection =>
            await connection.ExecuteAsync(new CommandDefinition(sql, opcion, cancellationToken: cancellationToken)));
    }

    public async Task DeactivateAsync(int id, int usuarioModifica, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE seg.Opcion
            SET Activo = 0, UsuarioModifica = @UsuarioModifica, FechaModifica = @FechaModifica
            WHERE Id = @Id;
            """;

        await ExecuteAsync(async connection =>
            await connection.ExecuteAsync(new CommandDefinition(
                sql,
                new { Id = id, UsuarioModifica = usuarioModifica, FechaModifica = DateTime.UtcNow },
                cancellationToken: cancellationToken)));
    }

    public async Task<IReadOnlyList<int>> GetAncestorIdsAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            WITH Ancestros AS (
                SELECT Id, IdPadre FROM seg.Opcion WHERE Id = @Id
                UNION ALL
                SELECT o.Id, o.IdPadre
                FROM seg.Opcion o
                INNER JOIN Ancestros a ON o.Id = a.IdPadre
            )
            SELECT Id FROM Ancestros;
            """;

        return await ExecuteAsync(async connection =>
            (await connection.QueryAsync<int>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken))).AsList());
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
            throw new DatabaseException("Ocurrió un error al acceder a la información de opciones.", ex);
        }
    }
}
