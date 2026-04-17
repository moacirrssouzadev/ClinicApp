using ClinicApp.Application.Dtos;
using ClinicApp.Domain.Core;

namespace ClinicApp.Application.Services;

/// <summary>
/// Interface para o serviço de Agendamentos
/// </summary>
public interface IAppointmentService
{
    Task<Result<AppointmentDto>> CreateAsync(CreateAppointmentDto dto);
    Task<Result<AppointmentDto>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<AppointmentDto>>> GetAllAsync();
    Task<Result<IEnumerable<AppointmentDto>>> GetByPatientIdAsync(Guid patientId);
    Task<Result<IEnumerable<AppointmentDto>>> GetByHealthProfessionalIdAsync(Guid healthProfessionalId);
    Task<Result<ProfessionalScheduleDto>> GetProfessionalScheduleAsync(Guid healthProfessionalId, DateTime date);
    Task<Result<IEnumerable<AvailableTimeSlotDto>>> GetAvailableTimeSlotsAsync(Guid healthProfessionalId, DateTime date);
    Task<Result> CompleteAsync(Guid id, string? notes = null);
    Task<Result> CancelAsync(Guid id, string? notes = null);
    Task<Result> MarkAsNoShowAsync(Guid id, string? notes = null);
    Task<Result> DeleteAsync(Guid id);
}
