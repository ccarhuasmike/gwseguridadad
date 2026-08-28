namespace Security.Application.Common.Interfaces;

public interface IUsuarioRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
