using Xunit;
using Moq;
using ClinicApp.Application.Services.Implementations;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.ValueObjects;
using ClinicApp.Application.Dtos;

namespace ClinicApp.Tests.Application.Services;

public class PatientServiceErrorHandlingTests
{
    private readonly Mock<IPatientRepository> _mockRepository;
    private readonly PatientService _service;

    public PatientServiceErrorHandlingTests()
    {
        _mockRepository = new Mock<IPatientRepository>();
        _service = new PatientService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrowsException_ShouldReturnFailure()
    {
        // Arrange
        var dto = new CreatePatientDto("Test", "123.456.789-10", "test@test.com", "11999999999", DateTime.Now.AddYears(-20), "Address");
        _mockRepository.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("erro", result.Message.ToLower());
    }
}