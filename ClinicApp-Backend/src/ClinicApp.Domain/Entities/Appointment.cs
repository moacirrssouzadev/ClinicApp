using ClinicApp.Domain.Core;
using ClinicApp.Domain.Exceptions;

namespace ClinicApp.Domain.Entities;

/// <summary>
/// Status possíveis para um agendamento
/// </summary>
public enum AppointmentStatus
{
    Scheduled = 1,
    Completed = 2,
    Cancelled = 3,
    NoShow = 4
}

/// <summary>
/// Agregado raiz: Agendamento
/// </summary>
public class Appointment : Entity
{
    public Guid PatientId { get; private set; }
    public Guid HealthProfessionalId { get; private set; }
    public DateTime AppointmentDate { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public Appointment(
        Guid patientId,
        Guid healthProfessionalId,
        DateTime appointmentDate,
        TimeSpan startTime) : base()
    {
        ValidateAppointmentDate(appointmentDate);
        ValidateTime(startTime);

        PatientId = patientId;
        HealthProfessionalId = healthProfessionalId;
        AppointmentDate = appointmentDate.Date;
        StartTime = startTime;
        EndTime = startTime.Add(TimeSpan.FromMinutes(30));
        Status = AppointmentStatus.Scheduled;
    }

    public Appointment(
        Guid id,
        Guid patientId,
        Guid healthProfessionalId,
        DateTime appointmentDate,
        TimeSpan startTime,
        TimeSpan endTime,
        AppointmentStatus status,
        string? notes,
        DateTime createdAt,
        DateTime? updatedAt) : base(id)
    {
        PatientId = patientId;
        HealthProfessionalId = healthProfessionalId;
        AppointmentDate = appointmentDate;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
        Notes = notes;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public void Complete(string? notes = null)
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new OperationNotAllowedException($"Apenas agendamentos marcados podem ser concluídos. Status atual: {Status}");

        Status = AppointmentStatus.Completed;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string? notes = null)
    {
        if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
            throw new OperationNotAllowedException($"Agendamento com status {Status} não pode ser cancelado.");

        Status = AppointmentStatus.Cancelled;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsNoShow(string? notes = null)
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new OperationNotAllowedException($"Apenas agendamentos marcados podem ser marcados como não apresentado. Status atual: {Status}");

        Status = AppointmentStatus.NoShow;
        Notes = notes ?? "Paciente não compareceu";
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsConflictingWith(Appointment other)
    {
        if (HealthProfessionalId != other.HealthProfessionalId)
            return false;

        if (AppointmentDate != other.AppointmentDate)
            return false;

        // Verifica se há sobreposição de horários
        return !(EndTime <= other.StartTime || StartTime >= other.EndTime);
    }

    public bool IsSameDayAsOtherPatient(Appointment other)
    {
        if (PatientId != other.PatientId)
            return false;

        if (HealthProfessionalId != other.HealthProfessionalId)
            return false;

        return AppointmentDate == other.AppointmentDate && Status == AppointmentStatus.Scheduled;
    }

    private static void ValidateAppointmentDate(DateTime appointmentDate)
    {
        var date = appointmentDate.Date;
        var dayOfWeek = date.DayOfWeek;

        if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
            throw new BusinessRuleException("Agendamentos não são permitidos nos fins de semana.");

        if (date < DateTime.Now.Date)
            throw new BusinessRuleException("Não é possível agendar uma consulta no passado.");
    }

    private static void ValidateTime(TimeSpan time)
    {
        var startHour = TimeSpan.FromHours(8);
        var endHour = TimeSpan.FromHours(18);

        if (time < startHour || time >= endHour)
            throw new BusinessRuleException("Horário deve estar entre 08:00 e 18:00.");

        // Valida que o horário é em intervalos de 30 minutos
        if (time.Minutes % 30 != 0)
            throw new BusinessRuleException("Horário deve ser em intervalos de 30 minutos.");
    }
}
