using ClinicApp.Application.Dtos;
using ClinicApp.Domain.Core;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Exceptions;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.ValueObjects;

namespace ClinicApp.Application.Services.Implementations;

/// <summary>
/// Implementação do serviço de Profissionais de Saúde
/// </summary>
public class HealthProfessionalService : IHealthProfessionalService
{
    private readonly IHealthProfessionalRepository _repository;

    public HealthProfessionalService(IHealthProfessionalRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Result<HealthProfessionalDto>> CreateAsync(CreateHealthProfessionalDto dto)
    {
        try
        {
            var existingByCpf = await _repository.GetByCpfAsync(dto.Cpf);
            if (existingByCpf != null)
                return Result.Failure<HealthProfessionalDto>("Já existe um profissional com este CPF.");

            var existingByEmail = await _repository.GetByEmailAsync(dto.Email);
            if (existingByEmail != null)
                return Result.Failure<HealthProfessionalDto>("Já existe um profissional com este email.");

            var existingByLicense = await _repository.GetByLicenseNumberAsync(dto.LicenseNumber);
            if (existingByLicense != null)
                return Result.Failure<HealthProfessionalDto>("Já existe um profissional com este número de registro.");

            var cpf = new Cpf(dto.Cpf);
            var email = new Email(dto.Email);
            var phone = new Phone(dto.Phone);
            var specialization = new Specialization(dto.Specialization);

            var professional = new HealthProfessional(
                dto.Name,
                cpf,
                email,
                phone,
                specialization,
                dto.LicenseNumber
            );

            await _repository.AddAsync(professional);

            return Result.Success(MapToDto(professional), "Profissional criado com sucesso.");
        }
        catch (ClinicApp.Domain.Exceptions.InvalidDataException ex)
        {
            return Result.Failure<HealthProfessionalDto>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<HealthProfessionalDto>($"Erro ao criar profissional: {ex.Message}");
        }
    }

    public async Task<Result<HealthProfessionalDto>> UpdateAsync(Guid id, UpdateHealthProfessionalDto dto)
    {
        try
        {
            var professional = await _repository.GetByIdAsync(id);
            if (professional == null)
                return Result.Failure<HealthProfessionalDto>("Profissional não encontrado.");

            var email = new Email(dto.Email);
            var phone = new Phone(dto.Phone);
            var specialization = new Specialization(dto.Specialization);

            professional.Update(dto.Name, email, phone, specialization);
            
            if (dto.IsActive && !professional.IsActive) professional.Activate();
            else if (!dto.IsActive && professional.IsActive) professional.Deactivate();

            await _repository.UpdateAsync(professional);

            return Result.Success(MapToDto(professional), "Profissional atualizado com sucesso.");
        }
        catch (ClinicApp.Domain.Exceptions.InvalidDataException ex)
        {
            return Result.Failure<HealthProfessionalDto>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<HealthProfessionalDto>($"Erro ao atualizar profissional: {ex.Message}");
        }
    }

    public async Task<Result<HealthProfessionalDto>> GetByIdAsync(Guid id)
    {
        var professional = await _repository.GetByIdAsync(id);
        if (professional == null)
            return Result.Failure<HealthProfessionalDto>("Profissional não encontrado.");

        return Result.Success(MapToDto(professional));
    }

    public async Task<Result<HealthProfessionalDto>> GetByCpfAsync(string cpf)
    {
        var professional = await _repository.GetByCpfAsync(cpf);
        if (professional == null)
            return Result.Failure<HealthProfessionalDto>("Profissional não encontrado.");

        return Result.Success(MapToDto(professional));
    }

    public async Task<Result<HealthProfessionalDto>> GetByEmailAsync(string email)
    {
        var professional = await _repository.GetByEmailAsync(email);
        if (professional == null)
            return Result.Failure<HealthProfessionalDto>("Profissional não encontrado.");

        return Result.Success(MapToDto(professional));
    }

    public async Task<Result<HealthProfessionalDto>> GetByLicenseNumberAsync(string licenseNumber)
    {
        var professional = await _repository.GetByLicenseNumberAsync(licenseNumber);
        if (professional == null)
            return Result.Failure<HealthProfessionalDto>("Profissional não encontrado.");

        return Result.Success(MapToDto(professional));
    }

    public async Task<Result<IEnumerable<HealthProfessionalDto>>> GetAllAsync()
    {
        var professionals = await _repository.GetAllAsync();
        return Result.Success(professionals.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<HealthProfessionalDto>>> GetActiveAsync()
    {
        var professionals = await _repository.GetActiveAsync();
        return Result.Success(professionals.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<HealthProfessionalDto>>> GetBySpecializationAsync(string specialization)
    {
        var professionals = await _repository.GetBySpecializationAsync(specialization);
        return Result.Success(professionals.Select(MapToDto));
    }

    public async Task<Result> DeactivateAsync(Guid id)
    {
        try
        {
            var professional = await _repository.GetByIdAsync(id);
            if (professional == null)
                return Result.Failure("Profissional não encontrado.");

            professional.Deactivate();
            await _repository.UpdateAsync(professional);

            return Result.Success("Profissional desativado com sucesso.");
        }
        catch (OperationNotAllowedException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao desativar profissional: {ex.Message}");
        }
    }

    public async Task<Result> ActivateAsync(Guid id)
    {
        try
        {
            var professional = await _repository.GetByIdAsync(id);
            if (professional == null)
                return Result.Failure("Profissional não encontrado.");

            professional.Activate();
            await _repository.UpdateAsync(professional);

            return Result.Success("Profissional ativado com sucesso.");
        }
        catch (OperationNotAllowedException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao ativar profissional: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            await _repository.DeleteAsync(id);
            return Result.Success("Profissional deletado com sucesso.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao deletar profissional: {ex.Message}");
        }
    }

    private static HealthProfessionalDto MapToDto(HealthProfessional professional)
    {
        return new HealthProfessionalDto(
            professional.Id,
            professional.Name,
            professional.Cpf.Number,
            professional.Email.Address,
            professional.Phone.Number,
            professional.Specialization.Name,
            professional.LicenseNumber,
            professional.IsActive,
            professional.CreatedAt,
            professional.UpdatedAt
        );
    }
}
