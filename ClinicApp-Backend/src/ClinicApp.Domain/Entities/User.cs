using ClinicApp.Domain.Core;
using ClinicApp.Domain.ValueObjects;

namespace ClinicApp.Domain.Entities;

/// <summary>
/// Agregado raiz: Usuário (Autenticação)
/// Representa um usuário do sistema que pode fazer login
/// </summary>
public class User : Entity
{
    public string Username { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FullName { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    public User(
        string username,
        Email email,
        string passwordHash,
        string fullName,
        UserRole role) : base()
    {
        ValidateUsername(username);
        ValidateFullName(fullName);

        Username = username;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        FullName = fullName;
        Role = role;
        IsActive = true;
    }

    public User(
        Guid id,
        string username,
        Email email,
        string passwordHash,
        string fullName,
        UserRole role,
        bool isActive,
        DateTime? lastLoginAt,
        DateTime createdAt,
        DateTime? updatedAt,
        string? refreshToken = null,
        DateTime? refreshTokenExpiryTime = null) : base(id)
    {
        ValidateUsername(username);
        ValidateFullName(fullName);

        Username = username;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        FullName = fullName;
        Role = role;
        IsActive = isActive;
        LastLoginAt = lastLoginAt;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = refreshTokenExpiryTime;
    }

    /// <summary>
    /// Atualiza o refresh token do usuário
    /// </summary>
    public void UpdateRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Limpa o refresh token do usuário (logout)
    /// </summary>
    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Atualiza a data do último login
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Atualiza a senha do usuário
    /// </summary>
    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ativa ou desativa o usuário
    /// </summary>
    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (username.Length < 3 || username.Length > 50)
            throw new ArgumentException("Username must be between 3 and 50 characters", nameof(username));

        if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9._-]+$"))
            throw new ArgumentException("Username can only contain letters, numbers, dots, underscores and hyphens", nameof(username));
    }

    private static void ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty", nameof(fullName));

        if (fullName.Length < 3 || fullName.Length > 150)
            throw new ArgumentException("Full name must be between 3 and 150 characters", nameof(fullName));
    }
}

/// <summary>
/// Enum para as roles de usuário no sistema
/// </summary>
public enum UserRole
{
    Admin = 0,
    HealthProfessional = 1,
    Patient = 2,
    Receptionist = 3
}
