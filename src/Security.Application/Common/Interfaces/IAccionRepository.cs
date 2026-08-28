using Security.Domain.Entities;

namespace Security.Application.Common.Interfaces;

public interface IAccionRepository
{
    Task<Accion?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Accion>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Accion>> GetByOpcionAsync(int idOpcion, bool includeInactive, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Checks that the given Accion belongs to the given Opcion.</summary>
    Task<bool> BelongsToOpcionAsync(int idAccion, int idOpcion, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(Accion accion, CancellationToken cancellationToken = default);

    Task UpdateAsync(Accion accion, CancellationToken cancellationToken = default);

    Task DeactivateAsync(int id, int usuarioModifica, CancellationToken cancellationToken = default);
}
