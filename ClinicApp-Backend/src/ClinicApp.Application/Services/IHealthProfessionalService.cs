using ClinicApp.Application.Dtos;
using ClinicApp.Domain.Core;

namespace ClinicApp.Application.Services;

/// <summary>
/// Interface para o serviço de Profissionais de Saúde
/// </summary>
public interface IHealthProfessionalService
{
    Task<Result<HealthProfessionalDto>> CreateAsync(CreateHealthProfessionalDto dto);
    Task<Result<HealthProfessionalDto>> UpdateAsync(Guid id, UpdateHealthProfessionalDto dto);
    Task<Result<HealthProfessionalDto>> GetByIdAsync(Guid id);
    Task<Result<HealthProfessionalDto>> GetByCpfAsync(string cpf);
    Task<Result<HealthProfessionalDto>> GetByEmailAsync(string email);
    Task<Result<HealthProfessionalDto>> GetByLicenseNumberAsync(string licenseNumber);
    Task<Result<IEnumerable<HealthProfessionalDto>>> GetAllAsync();
    Task<Result<IEnumerable<HealthProfessionalDto>>> GetActiveAsync();
    Task<Result<IEnumerable<HealthProfessionalDto>>> GetBySpecializationAsync(string specialization);
    Task<Result> DeactivateAsync(Guid id);
    Task<Result> ActivateAsync(Guid id);
    Task<Result> DeleteAsync(Guid id);
}
