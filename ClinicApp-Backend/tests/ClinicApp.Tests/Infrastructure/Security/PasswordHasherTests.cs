using ClinicApp.Infrastructure.Security;
using Xunit;

namespace ClinicApp.Tests.Infrastructure.Security;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher;

    public PasswordHasherTests()
    {
        _hasher = new PasswordHasher();
    }

    [Fact]
    public void VerifyPassword_WithInvalidBase64_ShouldReturnFalse()
    {
        // Arrange
        var password = "password123";
        var invalidHash = "not-a-base64-string!!!";

        // Act
        var result = _hasher.VerifyPassword(password, invalidHash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_WithEmptyHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "password123";
        var emptyHash = "";

        // Act
        var result = _hasher.VerifyPassword(password, emptyHash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_WithInvalidLengthHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "password123";
        // Base64 para 10 bytes (deveria ser 36: 16 salt + 20 hash)
        var invalidLengthHash = Convert.ToBase64String(new byte[10]);

        // Act
        var result = _hasher.VerifyPassword(password, invalidLengthHash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HashAndVerify_WithValidPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "SecurePassword123!";
        var hash = _hasher.HashPassword(password);

        // Act
        var result = _hasher.VerifyPassword(password, hash);

        // Assert
        Assert.True(result);
    }
}
