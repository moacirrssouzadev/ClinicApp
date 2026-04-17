using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ClinicApp.Infrastructure.Persistence;

/// <summary>
/// Fábrica para conexões com o SQL Server
/// </summary>
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

/// <summary>
/// Implementação da fábrica de conexões
/// </summary>
public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration), "ConnectionString 'DefaultConnection' não foi encontrada.");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
