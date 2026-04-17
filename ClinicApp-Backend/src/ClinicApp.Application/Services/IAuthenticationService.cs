using ClinicApp.Application.Dtos;
using ClinicApp.Domain.Core;

namespace ClinicApp.Application.Services;

/// <summary>
/// Interface para o serviço de Autenticação
/// </summary>
public interface IAuthenticationService
{
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
    Task<Result> RegisterAsync(RegisterUserDto dto);
}
