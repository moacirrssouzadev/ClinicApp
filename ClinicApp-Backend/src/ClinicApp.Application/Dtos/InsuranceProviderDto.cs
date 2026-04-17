namespace ClinicApp.Application.Dtos;

/// <summary>
/// DTO para representar um Convênio Médico
/// </summary>
public record InsuranceProviderDto(
    Guid Id,
    string Name,
    string Code,
    bool IsActive);

/// <summary>
/// DTO para criação de um Convênio Médico
/// </summary>
public record CreateInsuranceProviderDto(
    string Name,
    string Code);
