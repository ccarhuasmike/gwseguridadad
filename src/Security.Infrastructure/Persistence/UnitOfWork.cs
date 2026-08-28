using System.Data;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Infrastructure.Persistence;

/// <summary>
/// Dapper-based implementation of <see cref="IUnitOfWork"/>. Opens a single
/// connection, begins a SQL transaction, executes the requested operation and
/// commits it; any exception triggers a rollback before being rethrown.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task ExecuteAsync(Func<IDbConnection, IDbTransaction, Task> operation, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync<object?>(async (connection, transaction) =>
        {
            await operation(connection, transaction);
            return null;
        }, cancellationToken);
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<IDbConnection, IDbTransaction, Task<TResult>> operation, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();

        if (connection is System.Data.Common.DbConnection dbConnection)
        {
            await dbConnection.OpenAsync(cancellationToken);
        }
        else
        {
            connection.Open();
        }

        using var transaction = connection.BeginTransaction();

        try
        {
            var result = await operation(connection, transaction);
            transaction.Commit();
            return result;
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            if (ex is AppExceptionBase)
            {
                throw;
            }

            throw new DatabaseException("Ocurrió un error al ejecutar la operación transaccional.", ex);
        }
    }
}
