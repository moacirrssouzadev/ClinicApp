using ClinicApp.Domain.Core;
using ClinicApp.Domain.Exceptions;
using ClinicApp.Domain.ValueObjects;

namespace ClinicApp.Domain.Entities;

/// <summary>
/// Agregado raiz: Paciente
/// </summary>
public class Patient : Entity
{
    public string Name { get; private set; }
    public Cpf Cpf { get; private set; }
    public Email Email { get; private set; }
    public Phone Phone { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string Address { get; private set; }
    public bool IsActive { get; private set; }

    public Patient(
        string name,
        Cpf cpf,
        Email email,
        Phone phone,
        DateTime dateOfBirth,
        string address) : base()
    {
        ValidateName(name);
        ValidateDateOfBirth(dateOfBirth);
        ValidateAddress(address);

        Name = name;
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        DateOfBirth = dateOfBirth;
        Address = address;
        IsActive = true;
    }

    public Patient(
        Guid id,
        string name,
        Cpf cpf,
        Email email,
        Phone phone,
        DateTime dateOfBirth,
        string address,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt) : base(id)
    {
        Name = name;
        Cpf = cpf;
        Email = email;
        Phone = phone;
        DateOfBirth = dateOfBirth;
        Address = address;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public void Update(
        string name,
        Email email,
        Phone phone,
        string address)
    {
        ValidateName(name);
        ValidateAddress(address);

        Name = name;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new OperationNotAllowedException("Paciente já está inativo.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new OperationNotAllowedException("Paciente já está ativo.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new System.IO.InvalidDataException("Nome do paciente não pode ser vazio.");

        if (name.Length < 3 || name.Length > 150)
            throw new System.IO.InvalidDataException("Nome deve ter entre 3 e 150 caracteres.");
    }

    private static void ValidateDateOfBirth(DateTime dateOfBirth)
    {
        var age = DateTime.Now.Year - dateOfBirth.Year;

        if (age < 0 || age > 150)
            throw new System.IO.InvalidDataException("Data de nascimento inválida.");
    }

    private static void ValidateAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new System.IO.InvalidDataException("Endereço não pode ser vazio.");

        if (address.Length < 5 || address.Length > 250)
            throw new System.IO.InvalidDataException("Endereço deve ter entre 5 e 250 caracteres.");
    }
}
