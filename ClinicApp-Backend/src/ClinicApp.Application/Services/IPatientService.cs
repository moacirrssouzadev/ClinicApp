using ClinicApp.Application.Dtos;
using ClinicApp.Domain.Core;

namespace ClinicApp.Application.Services;

/// <summary>
/// Interface para o serviço de Pacientes
/// </summary>
public interface IPatientService
{
    Task<Result<PatientDto>> CreateAsync(CreatePatientDto dto);
    Task<Result<PatientDto>> UpdateAsync(Guid id, UpdatePatientDto dto);
    Task<Result<PatientDto>> GetByIdAsync(Guid id);
    Task<Result<PatientDto>> GetByCpfAsync(string cpf);
    Task<Result<PatientDto>> GetByEmailAsync(string email);
    Task<Result<IEnumerable<PatientDto>>> GetAllAsync();
    Task<Result<IEnumerable<PatientDto>>> GetActiveAsync();
    Task<Result> DeactivateAsync(Guid id);
    Task<Result> ActivateAsync(Guid id);
    Task<Result> DeleteAsync(Guid id);
}
