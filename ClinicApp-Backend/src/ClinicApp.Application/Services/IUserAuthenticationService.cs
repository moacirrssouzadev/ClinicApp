using ClinicApp.Application.Dtos;
using ClinicApp.Domain.Entities;

namespace ClinicApp.Application.Services;

/// <summary>
/// Interface para o serviço de autenticação de usuários
/// </summary>
public interface IUserAuthenticationService
{
    Task<LoginResponseDto> LoginAsync(LoginUserDto loginDto);
    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
    Task<UserDto?> RegisterAsync(CreateUserDto registerDto);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto?> GetUserByUsernameAsync(string username);
    Task<bool> UpdatePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<bool> UpdateUserAsync(Guid userId, UpdateUserDto updateDto);
    Task<bool> DeactivateUserAsync(Guid userId);
    Task LogoutAsync(Guid userId);
}
