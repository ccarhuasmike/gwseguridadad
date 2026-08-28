using System.Data;
using Security.Domain.Entities;

namespace Security.Application.Common.Interfaces;

public interface IPerfilRepository
{
    Task<Perfil?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Perfil>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodigoAsync(string codigo, int? excludeId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(Perfil perfil, CancellationToken cancellationToken = default);

    Task UpdateAsync(Perfil perfil, CancellationToken cancellationToken = default);

    Task DeactivateAsync(int id, int usuarioModifica, CancellationToken cancellationToken = default);
}
