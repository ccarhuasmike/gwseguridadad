using Dapper;
using Security.Application.Common.Interfaces;
using Security.Domain.Entities;
using Security.Domain.Exceptions;

namespace Security.Infrastructure.Repositories;

public class PerfilRepository : IPerfilRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PerfilRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Perfil?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, Codigo, Nombre, Descripcion, Activo, UsuarioRegistro, FechaRegistro, UsuarioModifica, FechaModifica
            FROM seg.Perfil
            WHERE Id = @Id;
            """;

        return await ExecuteAsync(async connection =>
            await connection.QuerySingleOrDefaultAsync<Perfil>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken)));
    }

    public async Task<IReadOnlyList<Perfil>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT Id, Codigo, Nombre, Descripcion, Activo, UsuarioRegistro, FechaRegistro, UsuarioModifica, FechaModifica
            FROM seg.Perfil
            {(includeInactive ? string.Empty : "WHERE Activo = 1")}
            ORDER BY Nombre;
            """;

        return await ExecuteAsync(async connection =>
            (await connection.QueryAsync<Perfil>(new CommandDefinition(sql, cancellationToken: cancellationToken))).AsList());
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM seg.Perfil WHERE Id = @Id;";

        return await ExecuteAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken)) > 0);
    }

    public async Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COUNT(1) FROM seg.Perfil
            WHERE Codigo = @Codigo AND (@ExcludeId IS NULL OR Id <> @ExcludeId);
            """;

        return await ExecuteAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, new { Codigo = codigo, ExcludeId = excludeId }, cancellationToken: cancellationToken)) > 0);
    }

    public async Task<int> CreateAsync(Perfil perfil, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO seg.Perfil (Codigo, Nombre, Descripcion, Activo, UsuarioRegistro, FechaRegistro)
            OUTPUT INSERTED.Id
            VALUES (@Codigo, @Nombre, @Descripcion, @Activo, @UsuarioRegistro, @FechaRegistro);
            """;

        return await ExecuteAsync(async connection =>
            await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, perfil, cancellationToken: cancellationToken)));
    }

    public async Task UpdateAsync(Perfil perfil, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE seg.Perfil
            SET Codigo = @Codigo,
                Nombre = @Nombre,
                Descripcion = @Descripcion,
                Activo = @Activo,
                UsuarioModifica = @UsuarioModifica,
                FechaModifica = @FechaModifica
            WHERE Id = @Id;
            """;

        await ExecuteAsync(async connection =>
            await connection.ExecuteAsync(new CommandDefinition(sql, perfil, cancellationToken: cancellationToken)));
    }

    public async Task DeactivateAsync(int id, int usuarioModifica, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE seg.Perfil
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
            throw new DatabaseException("Ocurrió un error al acceder a la información de perfiles.", ex);
        }
    }
}
