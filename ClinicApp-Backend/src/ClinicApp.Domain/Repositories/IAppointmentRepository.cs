using ClinicApp.Domain.Entities;

namespace ClinicApp.Domain.Repositories;

/// <summary>
/// Interface para o repositório de Agendamentos
/// </summary>
public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<Appointment>> GetByHealthProfessionalIdAsync(Guid healthProfessionalId);
    Task<IEnumerable<Appointment>> GetByHealthProfessionalAndDateAsync(Guid healthProfessionalId, DateTime date);
    Task<IEnumerable<Appointment>> GetByPatientAndHealthProfessionalAndDateAsync(Guid patientId, Guid healthProfessionalId, DateTime date);
    Task<IEnumerable<Appointment>> GetAllAsync();
    Task<IEnumerable<Appointment>> GetUpcomingAsync(DateTime fromDate);
    Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task AddAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task DeleteAsync(Guid id);
}
