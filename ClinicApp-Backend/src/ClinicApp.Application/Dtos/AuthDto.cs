namespace ClinicApp.Application.Dtos;

/// <summary>
/// DTO para registro de novo usuário
/// </summary>
public record RegisterUserDto(
    string Email,
    string Password,
    string Name);

/// <summary>
/// DTO para login
/// </summary>
public record LoginDto(
    string Email,
    string Password);

/// <summary>
/// DTO para refresh token
/// </summary>
public record RefreshTokenDto(
    string RefreshToken);

/// <summary>
/// DTO de resposta de autenticação
/// </summary>
public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresIn);

/// <summary>
/// DTO com informações do usuário autenticado
/// </summary>
public record UserClaimsDto(
    Guid UserId,
    string Email,
    string Name);
