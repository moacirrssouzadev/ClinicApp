namespace ClinicApp.Domain.Security;

/// <summary>
/// Interface para gerenciar senhas
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

/// <summary>
/// Interface para gerenciar tokens JWT
/// </summary>
public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string username, int role);
    string GenerateRefreshToken();
    System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
