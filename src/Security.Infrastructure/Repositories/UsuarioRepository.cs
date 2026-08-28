using Dapper;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UsuarioRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM seg.Usuario WHERE Id = @Id;";

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
            return count > 0;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Ocurrió un error al validar la existencia del usuario.", ex);
        }
    }
}
