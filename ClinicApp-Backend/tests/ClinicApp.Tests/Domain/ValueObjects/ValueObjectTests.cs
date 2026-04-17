using Xunit;
using ClinicApp.Domain.ValueObjects;
using ClinicApp.Domain.Exceptions;

namespace ClinicApp.Tests.Domain.ValueObjects;

public class ValueObjectTests
{
    [Fact]
    public void Email_WithValidEmail_ShouldCreate()
    {
        // Arrange & Act
        var email = new Email("test@example.com");

        // Assert
        Assert.Equal("test@example.com", email.Address);
    }

    [Fact]
    public void Email_WithInvalidEmail_ShouldThrow()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ClinicApp.Domain.Exceptions.InvalidDataException>(() => new Email("invalid-email"));
        Assert.Contains("Email", exception.Message);
    }

    [Fact]
    public void Cpf_WithValidCpf_ShouldCreate()
    {
        // Arrange & Act
        var cpf = new Cpf("123.456.789-10");

        // Assert
        Assert.Equal("123.456.789-10", cpf.Number);
    }

    [Fact]
    public void Cpf_WithInvalidCpf_ShouldThrow()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ClinicApp.Domain.Exceptions.InvalidDataException>(() => new Cpf("123"));
        Assert.Contains("11 dígitos", exception.Message);
    }

    [Fact]
    public void Phone_WithValidPhone_ShouldCreate()
    {
        // Arrange & Act
        var phone = new Phone("(11) 98765-4321");

        // Assert
        Assert.Contains("11", phone.Number);
    }

    [Fact]
    public void Specialization_WithValidSpecialization_ShouldCreate()
    {
        // Arrange & Act
        var specialization = new Specialization("Cardiologia");

        // Assert
        Assert.Equal("Cardiologia", specialization.Name);
    }

    [Fact]
    public void Specialization_WithTooShortName_ShouldThrow()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ClinicApp.Domain.Exceptions.InvalidDataException>(() => new Specialization("AB"));
        Assert.Contains("3 e 100 caracteres", exception.Message);
    }
}
