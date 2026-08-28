using System.Data;

namespace Security.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the creation of ADO.NET connections so that Application
/// handlers and Infrastructure repositories never depend directly on
/// Microsoft.Data.SqlClient.
/// </summary>
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
