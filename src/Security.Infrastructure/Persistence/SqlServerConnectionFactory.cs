using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Security.Application.Common.Interfaces;

namespace Security.Infrastructure.Persistence;

/// <summary>
/// Creates SQL Server ADO.NET connections using the connection string
/// configured in appsettings.json / environment variables. Never hardcode
/// connection strings; the value always comes from configuration.
/// </summary>
public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(IConfiguration configuration)
    {
        var baseConnectionString = configuration.GetConnectionString("SecurityDatabase")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'SecurityDatabase'. Configúrela en appsettings.json o mediante la variable de entorno ConnectionStrings__SecurityDatabase.");

        // The SQL authentication secret can optionally be supplied separately
        // (e.g. via the Db:SqlAuthSecret / Db__SqlAuthSecret environment variable),
        // so the credential never needs to be embedded inline in the
        // connection string configured in appsettings.json or docker-compose.yml.
        var sqlAuthSecret = configuration["Db:SqlAuthSecret"];
        if (!string.IsNullOrEmpty(sqlAuthSecret))
        {
            var builder = new SqlConnectionStringBuilder(baseConnectionString)
            {
                Password = sqlAuthSecret
            };
            _connectionString = builder.ConnectionString;
        }
        else
        {
            _connectionString = baseConnectionString;
        }
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
