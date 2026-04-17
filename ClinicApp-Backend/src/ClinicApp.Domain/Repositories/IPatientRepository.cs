using ClinicApp.Domain.Entities;

namespace ClinicApp.Domain.Repositories;

/// <summary>
/// Interface para o repositório de Pacientes
/// </summary>
public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(Guid id);
    Task<Patient?> GetByCpfAsync(string cpf);
    Task<Patient?> GetByEmailAsync(string email);
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<IEnumerable<Patient>> GetActiveAsync();
    Task AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(Guid id);
}
