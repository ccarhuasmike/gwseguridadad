using System.Data;

namespace Security.Application.Common.Interfaces;

/// <summary>
/// Coordinates operations that must be executed atomically against SQL Server
/// using a single connection/transaction pair (e.g. saving the full
/// Perfil/Usuario permission tree). Implemented with Dapper in Infrastructure.
/// </summary>
public interface IUnitOfWork
{
    Task ExecuteAsync(Func<IDbConnection, IDbTransaction, Task> operation, CancellationToken cancellationToken = default);

    Task<TResult> ExecuteAsync<TResult>(Func<IDbConnection, IDbTransaction, Task<TResult>> operation, CancellationToken cancellationToken = default);
}
