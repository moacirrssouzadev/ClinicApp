using System.Data;
using Dapper;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.ValueObjects;

namespace ClinicApp.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de Profissionais de Saúde usando Dapper
/// </summary>
public class HealthProfessionalRepository : IHealthProfessionalRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public HealthProfessionalRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<HealthProfessional?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Name, Cpf, Email, Phone, Specialization, LicenseNumber, IsActive, CreatedAt, UpdatedAt
            FROM HealthProfessionals
            WHERE Id = @Id";

        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { id });

        return result == null ? null : MapFromDynamic(result);
    }

    public async Task<HealthProfessional?> GetByCpfAsync(string cpf)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Name, Cpf, Email, Phone, Specialization, LicenseNumber, IsActive, CreatedAt, UpdatedAt
            FROM HealthProfessionals
            WHERE Cpf = @Cpf";

        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { cpf });

        return result == null ? null : MapFromDynamic(result);
    }

    public async Task<HealthProfessional?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Name, Cpf, Email, Phone, Specialization, LicenseNumber, IsActive, CreatedAt, UpdatedAt
            FROM HealthProfessionals
            WHERE Email = @Email";

        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { email });

        return result == null ? null : MapFromDynamic(result);
    }

    public async Task<HealthProfessional?> GetByLicenseNumberAsync(string licenseNumber)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Name, Cpf, Email, Phone, Specialization, LicenseNumber, IsActive, CreatedAt, UpdatedAt
            FROM HealthProfessionals
            WHERE LicenseNumber = @LicenseNumber";

        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { licenseNumber });

        return result == null ? null : MapFromDynamic(result);
    }

    public async Task<IEnumerable<HealthProfessional>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Name, Cpf, Email, Phone, Specialization, LicenseNumber, IsActive, CreatedAt, UpdatedAt
            FROM HealthProfessionals
            ORDER BY CreatedAt DESC";

        var results = await connection.QueryAsync<dynamic>(sql);

        return results.Select(MapFromDynamic).ToList();
    }

    public async Task<IEnumerable<HealthProfessional>> GetActiveAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Name, Cpf, Email, Phone, Specialization, LicenseNumber, IsActive, CreatedAt, UpdatedAt
            FROM HealthProfessionals
            WHERE IsActive = 1
            ORDER BY Name";

        var results = await connection.QueryAsync<dynamic>(sql);

        return results.Select(MapFromDynamic).ToList();
    }

    public async Task<IEnumerable<HealthProfessional>> GetBySpecializationAsync(string specialization)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Name, Cpf, Email, Phone, Specialization, LicenseNumber, IsActive, CreatedAt, UpdatedAt
            FROM HealthProfessionals
            WHERE Specialization = @Specialization AND IsActive = 1
            ORDER BY Name";

        var results = await connection.QueryAsync<dynamic>(sql, new { specialization });

        return results.Select(MapFromDynamic).ToList();
    }

    public async Task AddAsync(HealthProfessional healthProfessional)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO HealthProfessionals (Id, Name, Cpf, Email, Phone, Specialization, LicenseNumber, IsActive, CreatedAt, UpdatedAt)
            VALUES (@Id, @Name, @Cpf, @Email, @Phone, @Specialization, @LicenseNumber, @IsActive, @CreatedAt, @UpdatedAt)";

        await connection.ExecuteAsync(sql, new
        {
            healthProfessional.Id,
            healthProfessional.Name,
            Cpf = healthProfessional.Cpf.Number,
            Email = healthProfessional.Email.Address,
            Phone = healthProfessional.Phone.Number,
            Specialization = healthProfessional.Specialization.Name,
            healthProfessional.LicenseNumber,
            healthProfessional.IsActive,
            healthProfessional.CreatedAt,
            healthProfessional.UpdatedAt
        });
    }

    public async Task UpdateAsync(HealthProfessional healthProfessional)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE HealthProfessionals
            SET Name = @Name, Email = @Email, Phone = @Phone, Specialization = @Specialization, IsActive = @IsActive, UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        await connection.ExecuteAsync(sql, new
        {
            healthProfessional.Id,
            healthProfessional.Name,
            Email = healthProfessional.Email.Address,
            Phone = healthProfessional.Phone.Number,
            Specialization = healthProfessional.Specialization.Name,
            healthProfessional.IsActive,
            healthProfessional.UpdatedAt
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"DELETE FROM HealthProfessionals WHERE Id = @Id";

        await connection.ExecuteAsync(sql, new { id });
    }

    private static HealthProfessional MapFromDynamic(dynamic row)
    {
        return new HealthProfessional(
            (Guid)row.Id,
            (string)row.Name,
            Cpf.FromDatabase((string)row.Cpf),
            Email.FromDatabase((string)row.Email),
            Phone.FromDatabase((string)row.Phone),
            new Specialization((string)row.Specialization),
            (string)row.LicenseNumber,
            (bool)row.IsActive,
            (DateTime)row.CreatedAt,
            (DateTime?)row.UpdatedAt
        );
    }
}
