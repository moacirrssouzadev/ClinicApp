using Xunit;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Exceptions;
using ClinicApp.Domain.ValueObjects;

namespace ClinicApp.Tests.Domain.Entities;

public class AppointmentEntityTests
{
    [Fact]
    public void CreateAppointment_WithValidData_ShouldSucceed()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var professionalId = Guid.NewGuid();
        var appointmentDate = DateTime.Now.AddDays(5).Date;
        var startTime = TimeSpan.FromHours(10);

        // Act
        var appointment = new Appointment(patientId, professionalId, appointmentDate, startTime);

        // Assert
        Assert.NotEqual(Guid.Empty, appointment.Id);
        Assert.Equal(patientId, appointment.PatientId);
        Assert.Equal(professionalId, appointment.HealthProfessionalId);
        Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
        Assert.Equal(startTime, appointment.StartTime);
        Assert.Equal(startTime.Add(TimeSpan.FromMinutes(30)), appointment.EndTime);
    }

    [Fact]
    public void CreateAppointment_WithWeekendDate_ShouldFail()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var professionalId = Guid.NewGuid();
        var saturday = DateTime.Now.AddDays((6 - (int)DateTime.Now.DayOfWeek) % 7 == 0 ? 6 : (6 - (int)DateTime.Now.DayOfWeek) % 7).Date;
        var startTime = TimeSpan.FromHours(10);

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleException>(
            () => new Appointment(patientId, professionalId, saturday, startTime)
        );
        Assert.Contains("fins de semana", exception.Message);
    }

    [Fact]
    public void CreateAppointment_WithInvalidTime_ShouldFail()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var professionalId = Guid.NewGuid();
        var appointmentDate = DateTime.Now.AddDays(5).Date;
        var startTime = TimeSpan.FromHours(19); // Fora do horário permitido

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleException>(
            () => new Appointment(patientId, professionalId, appointmentDate, startTime)
        );
        Assert.Contains("08:00 e 18:00", exception.Message);
    }

    [Fact]
    public void CompleteAppointment_WithScheduledStatus_ShouldSucceed()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var professionalId = Guid.NewGuid();
        var appointmentDate = DateTime.Now.AddDays(5).Date;
        var appointment = new Appointment(patientId, professionalId, appointmentDate, TimeSpan.FromHours(10));

        // Act
        appointment.Complete("Consulta realizada com sucesso");

        // Assert
        Assert.Equal(AppointmentStatus.Completed, appointment.Status);
        Assert.Equal("Consulta realizada com sucesso", appointment.Notes);
    }

    [Fact]
    public void CancelAppointment_WithScheduledStatus_ShouldSucceed()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var professionalId = Guid.NewGuid();
        var appointmentDate = DateTime.Now.AddDays(5).Date;
        var appointment = new Appointment(patientId, professionalId, appointmentDate, TimeSpan.FromHours(10));

        // Act
        appointment.Cancel("Cancelado pelo paciente");

        // Assert
        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
    }
}
