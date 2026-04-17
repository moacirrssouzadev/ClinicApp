using System.Data;
using Dapper;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.ValueObjects;
using ClinicApp.Domain.Exceptions;

namespace ClinicApp.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de Pacientes usando Dapper
/// </summary>
public class PatientRepository : IPatientRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PatientRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<Patient?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Name, Cpf, Email, Phone, DateOfBirth, Address, IsActive, CreatedAt, UpdatedAt
            FROM Patients
            WHERE Id = @Id";

        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { id });

        return result == null ? null : MapFromDynamic(result);
    }

    public async Task<Patient?> GetByCpfAsync(string cpf)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Name, Cpf, Email, Phone, DateOfBirth, Address, IsActive, CreatedAt, UpdatedAt
            FROM Patients
            WHERE Cpf = @Cpf";

        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { cpf });

        return result == null ? null : MapFromDynamic(result);
    }

    public async Task<Patient?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Name, Cpf, Email, Phone, DateOfBirth, Address, IsActive, CreatedAt, UpdatedAt
            FROM Patients
            WHERE Email = @Email";

        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { email });

        return result == null ? null : MapFromDynamic(result);
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Name, Cpf, Email, Phone, DateOfBirth, Address, IsActive, CreatedAt, UpdatedAt
            FROM Patients
            ORDER BY CreatedAt DESC";

        var results = await connection.QueryAsync<dynamic>(sql);

        return results.Select(MapFromDynamic).ToList();
    }

    public async Task<IEnumerable<Patient>> GetActiveAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, Name, Cpf, Email, Phone, DateOfBirth, Address, IsActive, CreatedAt, UpdatedAt
            FROM Patients
            WHERE IsActive = 1
            ORDER BY Name";

        var results = await connection.QueryAsync<dynamic>(sql);

        return results.Select(MapFromDynamic).ToList();
    }

    public async Task AddAsync(Patient patient)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO Patients (Id, Name, Cpf, Email, Phone, DateOfBirth, Address, IsActive, CreatedAt, UpdatedAt)
            VALUES (@Id, @Name, @Cpf, @Email, @Phone, @DateOfBirth, @Address, @IsActive, @CreatedAt, @UpdatedAt)";

        await connection.ExecuteAsync(sql, new
        {
            patient.Id,
            patient.Name,
            Cpf = patient.Cpf.Number,
            Email = patient.Email.Address,
            Phone = patient.Phone.Number,
            patient.DateOfBirth,
            patient.Address,
            patient.IsActive,
            patient.CreatedAt,
            patient.UpdatedAt
        });
    }

    public async Task UpdateAsync(Patient patient)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE Patients
            SET Name = @Name, Email = @Email, Phone = @Phone, Address = @Address, IsActive = @IsActive, UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        await connection.ExecuteAsync(sql, new
        {
            patient.Id,
            patient.Name,
            Email = patient.Email.Address,
            Phone = patient.Phone.Number,
            patient.Address,
            patient.IsActive,
            patient.UpdatedAt
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"DELETE FROM Patients WHERE Id = @Id";

        await connection.ExecuteAsync(sql, new { id });
    }

    private static Patient MapFromDynamic(dynamic row)
    {
        return new Patient(
            (Guid)row.Id,
            (string)row.Name,
            Cpf.FromDatabase((string)row.Cpf),
            Email.FromDatabase((string)row.Email),
            Phone.FromDatabase((string)row.Phone),
            (DateTime)row.DateOfBirth,
            (string)row.Address,
            (bool)row.IsActive,
            (DateTime)row.CreatedAt,
            (DateTime?)row.UpdatedAt
        );
    }
}
