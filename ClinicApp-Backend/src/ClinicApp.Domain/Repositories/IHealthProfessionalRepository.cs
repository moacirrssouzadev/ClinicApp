using ClinicApp.Domain.Entities;

namespace ClinicApp.Domain.Repositories;

/// <summary>
/// Interface para o repositório de Profissionais de Saúde
/// </summary>
public interface IHealthProfessionalRepository
{
    Task<HealthProfessional?> GetByIdAsync(Guid id);
    Task<HealthProfessional?> GetByCpfAsync(string cpf);
    Task<HealthProfessional?> GetByEmailAsync(string email);
    Task<HealthProfessional?> GetByLicenseNumberAsync(string licenseNumber);
    Task<IEnumerable<HealthProfessional>> GetAllAsync();
    Task<IEnumerable<HealthProfessional>> GetActiveAsync();
    Task<IEnumerable<HealthProfessional>> GetBySpecializationAsync(string specialization);
    Task AddAsync(HealthProfessional healthProfessional);
    Task UpdateAsync(HealthProfessional healthProfessional);
    Task DeleteAsync(Guid id);
}
