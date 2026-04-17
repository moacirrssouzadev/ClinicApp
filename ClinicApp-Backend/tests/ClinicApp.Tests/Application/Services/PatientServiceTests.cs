using Xunit;
using Moq;
using ClinicApp.Application.Dtos;
using ClinicApp.Application.Services.Implementations;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.ValueObjects;

namespace ClinicApp.Tests.Application.Services;

public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _mockRepository;
    private readonly PatientService _service;

    public PatientServiceTests()
    {
        _mockRepository = new Mock<IPatientRepository>();
        _service = new PatientService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var dto = new CreatePatientDto(
            "João Silva",
            "123.456.789-10",
            "joao@test.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15),
            "Rua A, 123"
        );

        _mockRepository.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync((Patient?)null);
        _mockRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Patient?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("João Silva", result.Data.Name);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExistingCpf_ShouldReturnFailure()
    {
        // Arrange
        var dto = new CreatePatientDto(
            "João Silva",
            "123.456.789-10",
            "joao@test.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15),
            "Rua A, 123"
        );

        var existingPatient = new Patient(
            "Outro Paciente",
            new Cpf("123.456.789-10"),
            new Email("outro@test.com"),
            new Phone("(11) 98765-4321"),
            new DateTime(1985, 1, 1),
            "Rua B, 456"
        );

        _mockRepository.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync(existingPatient);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("CPF", result.Message);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnPatient()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patient = new Patient(
            patientId,
            "João Silva",
            new Cpf("123.456.789-10"),
            new Email("joao@test.com"),
            new Phone("(11) 98765-4321"),
            new DateTime(1990, 5, 15),
            "Rua A, 123",
            true,
            DateTime.UtcNow,
            null
        );

        _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(patient);

        // Act
        var result = await _service.GetByIdAsync(patientId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(patientId, result.Data.Id);
        Assert.Equal("João Silva", result.Data.Name);
    }

    [Fact]
    public async Task DeactivateAsync_WithExistingPatient_ShouldDeactivateSuccessfully()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patient = new Patient(
            patientId,
            "João Silva",
            new Cpf("123.456.789-10"),
            new Email("joao@test.com"),
            new Phone("(11) 98765-4321"),
            new DateTime(1990, 5, 15),
            "Rua A, 123",
            true,
            DateTime.UtcNow,
            null
        );

        _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(patient);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeactivateAsync(patientId);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Patient>()), Times.Once);
    }
}
