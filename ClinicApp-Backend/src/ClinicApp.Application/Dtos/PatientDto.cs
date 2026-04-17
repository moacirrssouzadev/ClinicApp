namespace ClinicApp.Application.Dtos;

/// <summary>
/// DTO para criação de Paciente
/// </summary>
public record CreatePatientDto(
    string Name,
    string Cpf,
    string Email,
    string Phone,
    DateTime DateOfBirth,
    string Address);

/// <summary>
/// DTO para atualização de Paciente
/// </summary>
public record UpdatePatientDto(
    string Name,
    string Email,
    string Phone,
    string Address);

/// <summary>
/// DTO de resposta para Paciente
/// </summary>
public record PatientDto(
    Guid Id,
    string Name,
    string Cpf,
    string Email,
    string Phone,
    DateTime DateOfBirth,
    string Address,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
