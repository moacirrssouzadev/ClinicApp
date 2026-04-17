namespace ClinicApp.Application.Dtos;

/// <summary>
/// DTO para criação de Profissional de Saúde
/// </summary>
public record CreateHealthProfessionalDto(
    string Name,
    string Cpf,
    string Email,
    string Phone,
    string Specialization,
    string LicenseNumber);

/// <summary>
/// DTO para atualização de Profissional de Saúde
/// </summary>
public record UpdateHealthProfessionalDto(
    string Name,
    string Email,
    string Phone,
    string Specialization,
    string LicenseNumber,
    bool IsActive);

/// <summary>
/// DTO de resposta para Profissional de Saúde
/// </summary>
public record HealthProfessionalDto(
    Guid Id,
    string Name,
    string Cpf,
    string Email,
    string Phone,
    string Specialization,
    string LicenseNumber,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
