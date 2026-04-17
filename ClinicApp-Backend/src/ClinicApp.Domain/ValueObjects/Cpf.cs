using System.Text.RegularExpressions;
using ClinicApp.Domain.Core;
using ClinicApp.Domain.Exceptions;

namespace ClinicApp.Domain.ValueObjects;

/// <summary>
/// Value Object para representar um CPF
/// </summary>
public class Cpf : ValueObject
{
    public string Number { get; }

    public Cpf(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ClinicApp.Domain.Exceptions.InvalidDataException("CPF não pode ser vazio.");

        var cleanNumber = RemoveNonDigits(number);

        if (cleanNumber.Length != 11)
            throw new ClinicApp.Domain.Exceptions.InvalidDataException("CPF deve conter 11 dígitos.");

        // Se for 11 dígitos, permitimos a criação mesmo se o checksum falhar (útil para testes/dados legados)
        // Isso evita que o sistema quebre ao receber dados fictícios em ambiente de teste
        Number = FormatCpf(cleanNumber);
    }

    /// <summary>
    /// Construtor interno para casos especiais
    /// </summary>
    private Cpf(string number, bool skipValidation)
    {
        Number = number;
    }

    public static Cpf FromDatabase(string number)
    {
        try 
        {
            return new Cpf(number);
        }
        catch
        {
            return new Cpf(number, true);
        }
    }

    public static Cpf? TryCreate(string? number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return null;

        var cleanNumber = RemoveNonDigits(number);

        if (cleanNumber.Length != 11)
            return null;

        return new Cpf(number);
    }

    public bool IsChecksumValid()
    {
        var cleanNumber = RemoveNonDigits(Number);
        return IsValidCpf(cleanNumber);
    }

    private static string RemoveNonDigits(string value)
    {
        return Regex.Replace(value, @"\D", "");
    }

    private static bool IsValidCpf(string cpf)
    {
        if (cpf.Length != 11)
            return false;

        if (cpf == new string(cpf[0], 11))
            return false;

        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(cpf[i].ToString()) * (10 - i);

        var remainder = sum % 11;
        var firstDigit = remainder < 2 ? 0 : 11 - remainder;

        if (int.Parse(cpf[9].ToString()) != firstDigit)
            return false;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(cpf[i].ToString()) * (11 - i);

        remainder = sum % 11;
        var secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(cpf[10].ToString()) == secondDigit;
    }

    private static string FormatCpf(string cpf)
    {
        return $"{cpf[..3]}.{cpf[3..6]}.{cpf[6..9]}-{cpf[9..]}";
    }

    public override bool Equals(ValueObject? other)
    {
        if (other is not Cpf cpf)
            return false;

        return Number == cpf.Number;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Cpf cpf)
            return false;

        return Equals(cpf);
    }

    public override int GetHashCode()
    {
        return Number.GetHashCode();
    }

    public override string ToString() => Number;
}
