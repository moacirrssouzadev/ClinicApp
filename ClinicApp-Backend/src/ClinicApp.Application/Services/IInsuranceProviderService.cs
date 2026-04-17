using ClinicApp.Application.Dtos;
using ClinicApp.Domain.Core;

namespace ClinicApp.Application.Services;

/// <summary>
/// Interface para o serviço de Convênios Médicos
/// </summary>
public interface IInsuranceProviderService
{
    Task<Result<IEnumerable<InsuranceProviderDto>>> GetAllAsync();
    Task<Result<InsuranceProviderDto>> GetByIdAsync(Guid id);
    Task<Result<InsuranceProviderDto>> CreateAsync(CreateInsuranceProviderDto dto);
    Task<Result> DeactivateAsync(Guid id);
}
