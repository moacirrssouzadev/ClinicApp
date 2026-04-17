using ClinicApp.Application.Dtos;
using ClinicApp.Domain.Core;
using ClinicApp.Domain.Entities;
using ClinicApp.Domain.Exceptions;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.ValueObjects;

namespace ClinicApp.Application.Services.Implementations;

/// <summary>
/// Implementação do serviço de Pacientes
/// </summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;

    public PatientService(IPatientRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Result<PatientDto>> CreateAsync(CreatePatientDto dto)
    {
        try
        {
            // Validar unicidade
            var existingByCpf = await _repository.GetByCpfAsync(dto.Cpf);
            if (existingByCpf != null)
                return Result.Failure<PatientDto>("Já existe um paciente com este CPF.");

            var existingByEmail = await _repository.GetByEmailAsync(dto.Email);
            if (existingByEmail != null)
                return Result.Failure<PatientDto>("Já existe um paciente com este email.");

            // Criar Value Objects
            var cpf = new Cpf(dto.Cpf);
            var email = new Email(dto.Email);
            var phone = new Phone(dto.Phone);

            // Criar entidade
            var patient = new Patient(
                dto.Name,
                cpf,
                email,
                phone,
                dto.DateOfBirth,
                dto.Address
            );

            // Salvar
            await _repository.AddAsync(patient);

            return Result.Success(MapToDto(patient), "Paciente criado com sucesso.");
        }
        catch (ClinicApp.Domain.Exceptions.InvalidDataException ex)
        {
            return Result.Failure<PatientDto>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<PatientDto>($"Erro ao criar paciente: {ex.Message}");
        }
    }

    public async Task<Result<PatientDto>> UpdateAsync(Guid id, UpdatePatientDto dto)
    {
        try
        {
            var patient = await _repository.GetByIdAsync(id);
            if (patient == null)
                return Result.Failure<PatientDto>("Paciente não encontrado.");

            var email = new Email(dto.Email);
            var phone = new Phone(dto.Phone);

            patient.Update(dto.Name, email, phone, dto.Address);
            await _repository.UpdateAsync(patient);

            return Result.Success(MapToDto(patient), "Paciente atualizado com sucesso.");
        }
        catch (ClinicApp.Domain.Exceptions.InvalidDataException ex)
        {
            return Result.Failure<PatientDto>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<PatientDto>($"Erro ao atualizar paciente: {ex.Message}");
        }
    }

    public async Task<Result<PatientDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var patient = await _repository.GetByIdAsync(id);
            if (patient == null)
                return Result.Failure<PatientDto>("Paciente não encontrado.");

            return Result.Success(MapToDto(patient));
        }
        catch (Exception ex)
        {
            return Result.Failure<PatientDto>($"Erro ao buscar paciente: {ex.Message}");
        }
    }

    public async Task<Result<PatientDto>> GetByCpfAsync(string cpf)
    {
        try
        {
            var patient = await _repository.GetByCpfAsync(cpf);
            if (patient == null)
                return Result.Failure<PatientDto>("Paciente não encontrado.");

            return Result.Success(MapToDto(patient));
        }
        catch (Exception ex)
        {
            return Result.Failure<PatientDto>($"Erro ao buscar paciente por CPF: {ex.Message}");
        }
    }

    public async Task<Result<PatientDto>> GetByEmailAsync(string email)
    {
        try
        {
            var patient = await _repository.GetByEmailAsync(email);
            if (patient == null)
                return Result.Failure<PatientDto>("Paciente não encontrado.");

            return Result.Success(MapToDto(patient));
        }
        catch (Exception ex)
        {
            return Result.Failure<PatientDto>($"Erro ao buscar paciente por email: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<PatientDto>>> GetAllAsync()
    {
        try
        {
            var patients = await _repository.GetAllAsync();
            return Result.Success(patients.Select(MapToDto));
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<PatientDto>>($"Erro ao listar pacientes: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<PatientDto>>> GetActiveAsync()
    {
        try
        {
            var patients = await _repository.GetActiveAsync();
            return Result.Success(patients.Select(MapToDto));
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<PatientDto>>($"Erro ao listar pacientes ativos: {ex.Message}");
        }
    }

    public async Task<Result> DeactivateAsync(Guid id)
    {
        try
        {
            var patient = await _repository.GetByIdAsync(id);
            if (patient == null)
                return Result.Failure("Paciente não encontrado.");

            patient.Deactivate();
            await _repository.UpdateAsync(patient);

            return Result.Success("Paciente desativado com sucesso.");
        }
        catch (OperationNotAllowedException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao desativar paciente: {ex.Message}");
        }
    }

    public async Task<Result> ActivateAsync(Guid id)
    {
        try
        {
            var patient = await _repository.GetByIdAsync(id);
            if (patient == null)
                return Result.Failure("Paciente não encontrado.");

            patient.Activate();
            await _repository.UpdateAsync(patient);

            return Result.Success("Paciente ativado com sucesso.");
        }
        catch (OperationNotAllowedException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao ativar paciente: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            await _repository.DeleteAsync(id);
            return Result.Success("Paciente deletado com sucesso.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao deletar paciente: {ex.Message}");
        }
    }

    private static PatientDto MapToDto(Patient patient)
    {
        return new PatientDto(
            patient.Id,
            patient.Name,
            patient.Cpf.Number,
            patient.Email.Address,
            patient.Phone.Number,
            patient.DateOfBirth,
            patient.Address,
            patient.IsActive,
            patient.CreatedAt,
            patient.UpdatedAt
        );
    }
}
