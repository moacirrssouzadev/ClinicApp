using ClinicApp.Application.Dtos;
using ClinicApp.Domain.Core;

namespace ClinicApp.Application.Services.Implementations;

/// <summary>
/// Implementação de exemplo para o serviço de Convênios Médicos
/// </summary>
public class InsuranceProviderService : IInsuranceProviderService
{
    private static readonly List<InsuranceProviderDto> _mockData = new()
    {
        new InsuranceProviderDto(Guid.NewGuid(), "Unimed", "UN001", true),
        new InsuranceProviderDto(Guid.NewGuid(), "Bradesco Saúde", "BS002", true),
        new InsuranceProviderDto(Guid.NewGuid(), "Amil", "AM003", true)
    };

    public async Task<Result<IEnumerable<InsuranceProviderDto>>> GetAllAsync()
    {
        return await Task.FromResult(Result.Success(_mockData.AsEnumerable()));
    }

    public async Task<Result<InsuranceProviderDto>> GetByIdAsync(Guid id)
    {
        var provider = _mockData.FirstOrDefault(p => p.Id == id);
        if (provider == null)
            return Result.Failure<InsuranceProviderDto>("Convênio não encontrado.");

        return await Task.FromResult(Result.Success(provider));
    }

    public async Task<Result<InsuranceProviderDto>> CreateAsync(CreateInsuranceProviderDto dto)
    {
        var newProvider = new InsuranceProviderDto(Guid.NewGuid(), dto.Name, dto.Code, true);
        _mockData.Add(newProvider);
        return await Task.FromResult(Result.Success(newProvider, "Convênio criado com sucesso."));
    }

    public async Task<Result> DeactivateAsync(Guid id)
    {
        var providerIndex = _mockData.FindIndex(p => p.Id == id);
        if (providerIndex == -1)
            return Result.Failure("Convênio não encontrado.");

        var provider = _mockData[providerIndex];
        _mockData[providerIndex] = provider with { IsActive = false };
        
        return await Task.FromResult(Result.Success("Convênio desativado com sucesso."));
    }
}
