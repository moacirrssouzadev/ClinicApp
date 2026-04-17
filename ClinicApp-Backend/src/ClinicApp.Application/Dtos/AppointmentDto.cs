namespace ClinicApp.Application.Dtos;

/// <summary>
/// DTO para criação de Agendamento
/// </summary>
public record CreateAppointmentDto(
    Guid PatientId,
    Guid HealthProfessionalId,
    DateTime AppointmentDate,
    TimeSpan StartTime);

/// <summary>
/// DTO de resposta para Agendamento
/// </summary>
public record AppointmentDto(
    Guid Id,
    Guid PatientId,
    Guid HealthProfessionalId,
    DateTime AppointmentDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    int Status,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

/// <summary>
/// DTO para agenda do profissional
/// </summary>
public record ProfessionalScheduleDto(
    Guid HealthProfessionalId,
    DateTime Date,
    List<AppointmentDto> Appointments);

/// <summary>
/// DTO para disponibilidade de horários
/// </summary>
public record AvailableTimeSlotDto(
    TimeSpan StartTime,
    TimeSpan EndTime,
    bool IsAvailable);
