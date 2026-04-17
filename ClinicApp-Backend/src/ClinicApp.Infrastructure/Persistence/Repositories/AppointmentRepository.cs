using System.Data;
using Dapper;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Repositories;

namespace ClinicApp.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de Agendamentos usando Dapper
/// </summary>
public class AppointmentRepository : IAppointmentRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AppointmentRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, PatientId, HealthProfessionalId, AppointmentDate, StartTime, EndTime, Status, Notes, CreatedAt, UpdatedAt
            FROM Appointments
            WHERE Id = @Id";

        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { id });

        return result == null ? null : MapFromDynamic(result);
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, PatientId, HealthProfessionalId, AppointmentDate, StartTime, EndTime, Status, Notes, CreatedAt, UpdatedAt
            FROM Appointments
            WHERE PatientId = @PatientId
            ORDER BY AppointmentDate DESC";

        var results = await connection.QueryAsync<dynamic>(sql, new { patientId });

        return results.Select(MapFromDynamic).ToList();
    }

    public async Task<IEnumerable<Appointment>> GetByHealthProfessionalIdAsync(Guid healthProfessionalId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, PatientId, HealthProfessionalId, AppointmentDate, StartTime, EndTime, Status, Notes, CreatedAt, UpdatedAt
            FROM Appointments
            WHERE HealthProfessionalId = @HealthProfessionalId
            ORDER BY AppointmentDate DESC";

        var results = await connection.QueryAsync<dynamic>(sql, new { healthProfessionalId });

        return results.Select(MapFromDynamic).ToList();
    }

    public async Task<IEnumerable<Appointment>> GetByHealthProfessionalAndDateAsync(Guid healthProfessionalId, DateTime date)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, PatientId, HealthProfessionalId, AppointmentDate, StartTime, EndTime, Status, Notes, CreatedAt, UpdatedAt
            FROM Appointments
            WHERE HealthProfessionalId = @HealthProfessionalId AND CAST(AppointmentDate AS DATE) = CAST(@Date AS DATE)
            ORDER BY StartTime";

        var results = await connection.QueryAsync<dynamic>(sql, new { healthProfessionalId, date });

        return results.Select(MapFromDynamic).ToList();
    }

    public async Task<IEnumerable<Appointment>> GetByPatientAndHealthProfessionalAndDateAsync(Guid patientId, Guid healthProfessionalId, DateTime date)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, PatientId, HealthProfessionalId, AppointmentDate, StartTime, EndTime, Status, Notes, CreatedAt, UpdatedAt
            FROM Appointments
            WHERE PatientId = @PatientId AND HealthProfessionalId = @HealthProfessionalId AND CAST(AppointmentDate AS DATE) = CAST(@Date AS DATE) AND Status = 1
            ORDER BY StartTime";

        var results = await connection.QueryAsync<dynamic>(sql, new { patientId, healthProfessionalId, date });

        return results.Select(MapFromDynamic).ToList();
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, PatientId, HealthProfessionalId, AppointmentDate, StartTime, EndTime, Status, Notes, CreatedAt, UpdatedAt
            FROM Appointments
            ORDER BY AppointmentDate DESC";

        var results = await connection.QueryAsync<dynamic>(sql);

        return results.Select(MapFromDynamic).ToList();
    }

    public async Task<IEnumerable<Appointment>> GetUpcomingAsync(DateTime fromDate)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, PatientId, HealthProfessionalId, AppointmentDate, StartTime, EndTime, Status, Notes, CreatedAt, UpdatedAt
            FROM Appointments
            WHERE AppointmentDate >= @FromDate AND Status = 1
            ORDER BY AppointmentDate, StartTime";

        var results = await connection.QueryAsync<dynamic>(sql, new { fromDate });

        return results.Select(MapFromDynamic).ToList();
    }

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT Id, PatientId, HealthProfessionalId, AppointmentDate, StartTime, EndTime, Status, Notes, CreatedAt, UpdatedAt
            FROM Appointments
            WHERE AppointmentDate BETWEEN @StartDate AND @EndDate
            ORDER BY AppointmentDate, StartTime";

        var results = await connection.QueryAsync<dynamic>(sql, new { startDate, endDate });

        return results.Select(MapFromDynamic).ToList();
    }

    public async Task AddAsync(Appointment appointment)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO Appointments (Id, PatientId, HealthProfessionalId, AppointmentDate, StartTime, EndTime, Status, Notes, CreatedAt, UpdatedAt)
            VALUES (@Id, @PatientId, @HealthProfessionalId, @AppointmentDate, @StartTime, @EndTime, @Status, @Notes, @CreatedAt, @UpdatedAt)";

        await connection.ExecuteAsync(sql, new
        {
            appointment.Id,
            appointment.PatientId,
            appointment.HealthProfessionalId,
            appointment.AppointmentDate,
            appointment.StartTime,
            appointment.EndTime,
            Status = (int)appointment.Status,
            appointment.Notes,
            appointment.CreatedAt,
            appointment.UpdatedAt
        });
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE Appointments
            SET Status = @Status, Notes = @Notes, UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        await connection.ExecuteAsync(sql, new
        {
            appointment.Id,
            Status = (int)appointment.Status,
            appointment.Notes,
            appointment.UpdatedAt
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"DELETE FROM Appointments WHERE Id = @Id";

        await connection.ExecuteAsync(sql, new { id });
    }

    private static Appointment MapFromDynamic(dynamic row)
    {
        return new Appointment(
            (Guid)row.Id,
            (Guid)row.PatientId,
            (Guid)row.HealthProfessionalId,
            (DateTime)row.AppointmentDate,
            (TimeSpan)row.StartTime,
            (TimeSpan)row.EndTime,
            (AppointmentStatus)(int)row.Status,
            (string?)row.Notes,
            (DateTime)row.CreatedAt,
            (DateTime?)row.UpdatedAt
        );
    }
}
