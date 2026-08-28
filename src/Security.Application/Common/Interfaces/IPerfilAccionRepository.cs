using System.Data;
using Security.Domain.Entities;

namespace Security.Application.Common.Interfaces;

public interface IPerfilAccionRepository
{
    Task<IReadOnlyList<PerfilAccion>> GetByPerfilAsync(int idPerfil, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int idPerfil, int idAccion, CancellationToken cancellationToken = default);

    Task AddAsync(PerfilAccion entity, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task RemoveAsync(int idPerfil, int idAccion, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task RemoveAllForPerfilAsync(int idPerfil, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
