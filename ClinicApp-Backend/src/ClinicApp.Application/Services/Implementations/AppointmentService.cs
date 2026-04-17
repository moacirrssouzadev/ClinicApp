using ClinicApp.Application.Dtos;
using ClinicApp.Domain.Core;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Exceptions;
using ClinicApp.Domain.Repositories;

namespace ClinicApp.Application.Services.Implementations;

/// <summary>
/// Implementação do serviço de Agendamentos
/// </summary>
public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IHealthProfessionalRepository _healthProfessionalRepository;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IPatientRepository patientRepository,
        IHealthProfessionalRepository healthProfessionalRepository)
    {
        _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        _healthProfessionalRepository = healthProfessionalRepository ?? throw new ArgumentNullException(nameof(healthProfessionalRepository));
    }

    public async Task<Result<AppointmentDto>> CreateAsync(CreateAppointmentDto dto)
    {
        try
        {
            // Validações de negócio
            var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
            if (patient == null)
                return Result.Failure<AppointmentDto>("Paciente não encontrado.");

            if (!patient.IsActive)
                return Result.Failure<AppointmentDto>("Paciente está inativo.");

            var professional = await _healthProfessionalRepository.GetByIdAsync(dto.HealthProfessionalId);
            if (professional == null)
                return Result.Failure<AppointmentDto>("Profissional não encontrado.");

            if (!professional.IsActive)
                return Result.Failure<AppointmentDto>("Profissional está inativo.");

            // Criar agendamento
            var appointment = new Appointment(
                dto.PatientId,
                dto.HealthProfessionalId,
                dto.AppointmentDate,
                dto.StartTime
            );

            // Verificar conflitos
            var existingAppointments = await _appointmentRepository.GetByHealthProfessionalAndDateAsync(
                dto.HealthProfessionalId,
                dto.AppointmentDate
            );

            foreach (var existing in existingAppointments)
            {
                if (existing.Status == AppointmentStatus.Scheduled && appointment.IsConflictingWith(existing))
                    return Result.Failure<AppointmentDto>("Horário não está disponível para o profissional.");
            }

            // Verificar regra: um paciente só pode ter 1 consulta por profissional por dia
            var sameDay = await _appointmentRepository.GetByPatientAndHealthProfessionalAndDateAsync(
                dto.PatientId,
                dto.HealthProfessionalId,
                dto.AppointmentDate
            );

            if (sameDay.Any())
                return Result.Failure<AppointmentDto>("Paciente já possui uma consulta agendada com este profissional neste dia.");

            // Salvar
            await _appointmentRepository.AddAsync(appointment);

            return Result.Success(MapToDto(appointment), "Agendamento criado com sucesso.");
        }
        catch (BusinessRuleException ex)
        {
            return Result.Failure<AppointmentDto>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentDto>($"Erro ao criar agendamento: {ex.Message}");
        }
    }

    public async Task<Result<AppointmentDto>> GetByIdAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
            return Result.Failure<AppointmentDto>("Agendamento não encontrado.");

        return Result.Success(MapToDto(appointment));
    }

    public async Task<Result<IEnumerable<AppointmentDto>>> GetAllAsync()
    {
        var appointments = await _appointmentRepository.GetAllAsync();
        return Result.Success(appointments.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<AppointmentDto>>> GetByPatientIdAsync(Guid patientId)
    {
        var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);
        return Result.Success(appointments.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<AppointmentDto>>> GetByHealthProfessionalIdAsync(Guid healthProfessionalId)
    {
        var appointments = await _appointmentRepository.GetByHealthProfessionalIdAsync(healthProfessionalId);
        return Result.Success(appointments.Select(MapToDto));
    }

    public async Task<Result<ProfessionalScheduleDto>> GetProfessionalScheduleAsync(Guid healthProfessionalId, DateTime date)
    {
        var appointments = await _appointmentRepository.GetByHealthProfessionalAndDateAsync(healthProfessionalId, date);

        var schedule = new ProfessionalScheduleDto(
            healthProfessionalId,
            date.Date,
            appointments.Select(MapToDto).ToList()
        );

        return Result.Success(schedule);
    }

    public async Task<Result<IEnumerable<AvailableTimeSlotDto>>> GetAvailableTimeSlotsAsync(Guid healthProfessionalId, DateTime date)
    {
        var appointments = await _appointmentRepository.GetByHealthProfessionalAndDateAsync(healthProfessionalId, date);

        var timeSlots = new List<AvailableTimeSlotDto>();

        // Gerar slots de 30 em 30 minutos de 08:00 às 18:00
        var startHour = TimeSpan.FromHours(8);
        var endHour = TimeSpan.FromHours(18);

        for (var time = startHour; time < endHour; time = time.Add(TimeSpan.FromMinutes(30)))
        {
            var isAvailable = !appointments.Any(a => 
                a.Status == AppointmentStatus.Scheduled && 
                a.StartTime == time
            );

            timeSlots.Add(new AvailableTimeSlotDto(
                time,
                time.Add(TimeSpan.FromMinutes(30)),
                isAvailable
            ));
        }

        return Result.Success(timeSlots.AsEnumerable());
    }

    public async Task<Result> CompleteAsync(Guid id, string? notes = null)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                return Result.Failure("Agendamento não encontrado.");

            appointment.Complete(notes);
            await _appointmentRepository.UpdateAsync(appointment);

            return Result.Success("Agendamento concluído com sucesso.");
        }
        catch (OperationNotAllowedException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao completar agendamento: {ex.Message}");
        }
    }

    public async Task<Result> CancelAsync(Guid id, string? notes = null)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                return Result.Failure("Agendamento não encontrado.");

            appointment.Cancel(notes);
            await _appointmentRepository.UpdateAsync(appointment);

            return Result.Success("Agendamento cancelado com sucesso.");
        }
        catch (OperationNotAllowedException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao cancelar agendamento: {ex.Message}");
        }
    }

    public async Task<Result> MarkAsNoShowAsync(Guid id, string? notes = null)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                return Result.Failure("Agendamento não encontrado.");

            appointment.MarkAsNoShow(notes);
            await _appointmentRepository.UpdateAsync(appointment);

            return Result.Success("Agendamento marcado como não apresentado.");
        }
        catch (OperationNotAllowedException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao marcar como não apresentado: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            await _appointmentRepository.DeleteAsync(id);
            return Result.Success("Agendamento deletado com sucesso.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao deletar agendamento: {ex.Message}");
        }
    }

    private static AppointmentDto MapToDto(Appointment appointment)
    {
        return new AppointmentDto(
            appointment.Id,
            appointment.PatientId,
            appointment.HealthProfessionalId,
            appointment.AppointmentDate,
            appointment.StartTime,
            appointment.EndTime,
            (int)appointment.Status,
            appointment.Notes,
            appointment.CreatedAt,
            appointment.UpdatedAt
        );
    }
}
