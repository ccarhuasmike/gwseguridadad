using Security.Domain.Entities;

namespace Security.Application.Common.Interfaces;

public interface IOpcionRepository
{
    Task<Opcion?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Opcion>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Opcion>> GetChildrenAsync(int idPadre, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> HasAccionesAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(Opcion opcion, CancellationToken cancellationToken = default);

    Task UpdateAsync(Opcion opcion, CancellationToken cancellationToken = default);

    Task DeactivateAsync(int id, int usuarioModifica, CancellationToken cancellationToken = default);

    /// <summary>Returns every ancestor id of the given option, following IdPadre up to the root. Used to detect cycles.</summary>
    Task<IReadOnlyList<int>> GetAncestorIdsAsync(int id, CancellationToken cancellationToken = default);
}
