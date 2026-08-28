using System.Data;
using Security.Domain.Entities;

namespace Security.Application.Common.Interfaces;

public interface IUsuarioOpcionRepository
{
    Task<IReadOnlyList<UsuarioOpcion>> GetByUsuarioAsync(int idUsuario, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int idUsuario, int idOpcion, CancellationToken cancellationToken = default);

    Task AddAsync(UsuarioOpcion entity, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task RemoveAsync(int idUsuario, int idOpcion, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task RemoveAllForUsuarioAsync(int idUsuario, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
