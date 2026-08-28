using System.Data;
using Security.Domain.Entities;

namespace Security.Application.Common.Interfaces;

public interface IPerfilOpcionRepository
{
    Task<IReadOnlyList<PerfilOpcion>> GetByPerfilAsync(int idPerfil, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int idPerfil, int idOpcion, CancellationToken cancellationToken = default);

    Task AddAsync(PerfilOpcion entity, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task RemoveAsync(int idPerfil, int idOpcion, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task RemoveAllForPerfilAsync(int idPerfil, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
