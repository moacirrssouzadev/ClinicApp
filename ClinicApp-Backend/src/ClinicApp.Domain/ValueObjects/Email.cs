using System.Text.RegularExpressions;
using ClinicApp.Domain.Core;
using ClinicApp.Domain.Exceptions;

namespace ClinicApp.Domain.ValueObjects;

/// <summary>
/// Value Object para representar um Email
/// </summary>
public class Email : ValueObject
{
    public string Address { get; }

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ClinicApp.Domain.Exceptions.InvalidDataException("Email não pode ser vazio.");

        if (!IsValidEmail(address))
            throw new ClinicApp.Domain.Exceptions.InvalidDataException("Email em formato inválido.");

        Address = address.ToLower();
    }

    /// <summary>
    /// Construtor interno para carregar dados do banco mesmo se forem inválidos
    /// </summary>
    private Email(string address, bool skipValidation)
    {
        Address = address;
    }

    public static Email FromDatabase(string address)
    {
        try 
        {
            return new Email(address);
        }
        catch
        {
            return new Email(address, true);
        }
    }

    private static bool IsValidEmail(string email)
    {
        var pattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
        return Regex.IsMatch(email, pattern);
    }

    public override bool Equals(ValueObject? other)
    {
        if (other is not Email email)
            return false;

        return Address == email.Address;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Email email)
            return false;

        return Equals(email);
    }

    public override int GetHashCode()
    {
        return Address.GetHashCode();
    }

    public override string ToString() => Address;
}
