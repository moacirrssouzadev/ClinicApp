using Xunit;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.ValueObjects;

namespace ClinicApp.Tests.Domain.Entities;

public class UserEntityTests
{
    [Fact]
    public void CreateUser_WithValidData_ShouldSucceed()
    {
        // Arrange
        var username = "testuser";
        var email = new Email("test@clinicapp.com");
        var passwordHash = "hashedpassword";
        var fullName = "Test User";
        var role = UserRole.Patient;

        // Act
        var user = new User(username, email, passwordHash, fullName, role);

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(username, user.Username);
        Assert.Equal(email, user.Email);
        Assert.Equal(fullName, user.FullName);
        Assert.Equal(role, user.Role);
        Assert.True(user.IsActive);
        Assert.Null(user.RefreshToken);
    }

    [Fact]
    public void UpdateRefreshToken_WithValidData_ShouldSucceed()
    {
        // Arrange
        var user = new User("testuser", new Email("test@clinicapp.com"), "hash", "Test User", UserRole.Patient);
        var refreshToken = "new-refresh-token";
        var expiryTime = DateTime.UtcNow.AddDays(7);

        // Act
        user.UpdateRefreshToken(refreshToken, expiryTime);

        // Assert
        Assert.Equal(refreshToken, user.RefreshToken);
        Assert.Equal(expiryTime, user.RefreshTokenExpiryTime);
    }

    [Fact]
    public void ClearRefreshToken_ShouldRemoveToken()
    {
        // Arrange
        var user = new User("testuser", new Email("test@clinicapp.com"), "hash", "Test User", UserRole.Patient);
        user.UpdateRefreshToken("token", DateTime.UtcNow.AddDays(1));

        // Act
        user.ClearRefreshToken();

        // Assert
        Assert.Null(user.RefreshToken);
        Assert.Null(user.RefreshTokenExpiryTime);
    }

    [Fact]
    public void UpdateLastLogin_ShouldUpdateTimestamps()
    {
        // Arrange
        var user = new User("testuser", new Email("test@clinicapp.com"), "hash", "Test User", UserRole.Patient);
        var beforeUpdate = DateTime.UtcNow;

        // Act
        user.UpdateLastLogin();

        // Assert
        Assert.NotNull(user.LastLoginAt);
        Assert.True(user.LastLoginAt >= beforeUpdate);
        Assert.NotNull(user.UpdatedAt);
        Assert.True(user.UpdatedAt >= beforeUpdate);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("thisusernameiswaytoolongtobevalidandshouldthrowanexception")]
    public void CreateUser_WithInvalidUsername_ShouldFail(string invalidUsername)
    {
        // Arrange
        var email = new Email("test@clinicapp.com");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new User(invalidUsername, email, "hash", "Test User", UserRole.Patient)
        );
    }
}
