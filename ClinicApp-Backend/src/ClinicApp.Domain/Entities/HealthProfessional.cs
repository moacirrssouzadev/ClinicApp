using ClinicApp.Domain.Core;
using ClinicApp.Domain.Exceptions;
using ClinicApp.Domain.ValueObjects;
using System.IO;

namespace ClinicApp.Domain.Entities;

/// <summary>
/// Agregado raiz: Profissional de Saúde
/// </summary>
public class HealthProfessional : Entity
{
    public string Name { get; private set; }
    public Cpf Cpf { get; private set; }
    public Email Email { get; private set; }
    public Phone Phone { get; private set; }
    public Specialization Specialization { get; private set; }
    public string LicenseNumber { get; private set; }
    public bool IsActive { get; private set; }

    public HealthProfessional(
        string name,
        Cpf cpf,
        Email email,
        Phone phone,
        Specialization specialization,
        string licenseNumber) : base()
    {
        ValidateName(name);
        ValidateLicenseNumber(licenseNumber);

        Name = name;
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        Specialization = specialization ?? throw new ArgumentNullException(nameof(specialization));
        LicenseNumber = licenseNumber;
        IsActive = true;
    }

    public HealthProfessional(
        Guid id,
        string name,
        Cpf cpf,
        Email email,
        Phone phone,
        Specialization specialization,
        string licenseNumber,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt) : base(id)
    {
        Name = name;
        Cpf = cpf;
        Email = email;
        Phone = phone;
        Specialization = specialization;
        LicenseNumber = licenseNumber;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public void Update(
        string name,
        Email email,
        Phone phone,
        Specialization specialization)
    {
        ValidateName(name);

        Name = name;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        Specialization = specialization ?? throw new ArgumentNullException(nameof(specialization));
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new OperationNotAllowedException("Profissional já está inativo.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new OperationNotAllowedException("Profissional já está ativo.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new System.IO.InvalidDataException("Nome do profissional não pode ser vazio.");

        if (name.Length < 3 || name.Length > 150)
            throw new System.IO.InvalidDataException("Nome deve ter entre 3 e 150 caracteres.");
    }

    private static void ValidateLicenseNumber(string licenseNumber)
    {
        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new System.IO.InvalidDataException("Número de registro não pode ser vazio.");

        if (licenseNumber.Length < 5 || licenseNumber.Length > 20)
            throw new System.IO.InvalidDataException("Número de registro deve ter entre 5 e 20 caracteres.");
    }
}
