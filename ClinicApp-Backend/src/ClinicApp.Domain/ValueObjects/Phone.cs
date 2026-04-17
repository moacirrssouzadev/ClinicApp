using System.Text.RegularExpressions;
using ClinicApp.Domain.Core;
using ClinicApp.Domain.Exceptions;

namespace ClinicApp.Domain.ValueObjects;

/// <summary>
/// Value Object para representar um Telefone
/// </summary>
public class Phone : ValueObject
{
    public string Number { get; }

    public Phone(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new System.IO.InvalidDataException("Telefone não pode ser vazio.");

        var cleanNumber = RemoveNonDigits(number);

        if (!IsValidPhone(cleanNumber))
            throw new System.IO.InvalidDataException("Telefone em formato inválido.");

        Number = FormatPhone(cleanNumber);
    }

    /// <summary>
    /// Construtor interno para carregar dados do banco mesmo se forem inválidos
    /// </summary>
    private Phone(string number, bool skipValidation)
    {
        Number = number;
    }

    public static Phone FromDatabase(string number)
    {
        try 
        {
            return new Phone(number);
        }
        catch
        {
            return new Phone(number, true);
        }
    }

    private static string RemoveNonDigits(string value)
    {
        return Regex.Replace(value, @"\D", "");
    }

    private static bool IsValidPhone(string phone)
    {
        return phone.Length >= 10 && phone.Length <= 11;
    }

    private static string FormatPhone(string phone)
    {
        if (phone.Length == 10)
            return $"({phone[..2]}) {phone[2..6]}-{phone[6..]}";

        return $"({phone[..2]}) {phone[2..7]}-{phone[7..]}";
    }

    public override bool Equals(ValueObject? other)
    {
        if (other is not Phone phone)
            return false;

        return Number == phone.Number;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Phone phone)
            return false;

        return Equals(phone);
    }

    public override int GetHashCode()
    {
        return Number.GetHashCode();
    }

    public override string ToString() => Number;
}
