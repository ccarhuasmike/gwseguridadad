using System.Data;
using Security.Domain.Entities;

namespace Security.Application.Common.Interfaces;

public interface IUsuarioAccionRepository
{
    Task<IReadOnlyList<UsuarioAccion>> GetByUsuarioAsync(int idUsuario, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int idUsuario, int idAccion, CancellationToken cancellationToken = default);

    Task AddAsync(UsuarioAccion entity, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task RemoveAsync(int idUsuario, int idAccion, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task RemoveAllForUsuarioAsync(int idUsuario, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
