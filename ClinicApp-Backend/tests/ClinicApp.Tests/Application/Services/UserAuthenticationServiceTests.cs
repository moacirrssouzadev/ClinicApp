using Xunit;
using Moq;
using ClinicApp.Application.Dtos;
using ClinicApp.Application.Services.Implementations;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.Security;
using ClinicApp.Domain.ValueObjects;

namespace ClinicApp.Tests.Application.Services;

public class UserAuthenticationServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly Mock<IPatientRepository> _mockPatientRepository;
    private readonly Mock<IHealthProfessionalRepository> _mockProfessionalRepository;
    private readonly UserAuthenticationService _service;

    public UserAuthenticationServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();
        _mockPatientRepository = new Mock<IPatientRepository>();
        _mockProfessionalRepository = new Mock<IHealthProfessionalRepository>();

        _service = new UserAuthenticationService(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockJwtTokenService.Object,
            _mockPatientRepository.Object,
            _mockProfessionalRepository.Object
        );
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        var loginDto = new LoginUserDto { Username = "testuser", Password = "password123" };
        var user = new User("testuser", new Email("test@clinicapp.com"), "hashed_password", "Test User", UserRole.Patient);
        
        _mockUserRepository.Setup(r => r.GetByUsernameAsync("testuser")).ReturnsAsync(user);
        _mockPasswordHasher.Setup(h => h.VerifyPassword("password123", "hashed_password")).Returns(true);
        _mockJwtTokenService.Setup(s => s.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>())).Returns("access_token");
        _mockJwtTokenService.Setup(s => s.GenerateRefreshToken()).Returns("refresh_token");

        // Act
        var result = await _service.LoginAsync(loginDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("access_token", result.Token);
        Assert.Equal("refresh_token", result.RefreshToken);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidToken_ShouldReturnNewTokens()
    {
        // Arrange
        var refreshToken = "valid_refresh_token";
        var user = new User("testuser", new Email("test@clinicapp.com"), "hash", "Test User", UserRole.Patient);
        user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(1));

        _mockUserRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { user });
        _mockJwtTokenService.Setup(s => s.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>())).Returns("new_access_token");
        _mockJwtTokenService.Setup(s => s.GenerateRefreshToken()).Returns("new_refresh_token");

        // Act
        var result = await _service.RefreshTokenAsync(refreshToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("new_access_token", result.Token);
        Assert.Equal("new_refresh_token", result.RefreshToken);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithExpiredToken_ShouldReturnFailure()
    {
        // Arrange
        var refreshToken = "expired_token";
        var user = new User("testuser", new Email("test@clinicapp.com"), "hash", "Test User", UserRole.Patient);
        user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(-1)); // Expirado ontem

        _mockUserRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { user });

        // Act
        var result = await _service.RefreshTokenAsync(refreshToken);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Refresh token inválido ou expirado", result.Message);
    }
}
