using ClinicApp.Domain.Core;
using ClinicApp.Domain.Exceptions;

namespace ClinicApp.Domain.ValueObjects;

/// <summary>
/// Value Object para representar a especialização de um profissional de saúde
/// </summary>
public class Specialization : ValueObject
{
    public string Name { get; }

    public Specialization(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ClinicApp.Domain.Exceptions.InvalidDataException("Especialização não pode ser vazia.");

        if (name.Length < 3 || name.Length > 100)
            throw new ClinicApp.Domain.Exceptions.InvalidDataException("Especialização deve ter entre 3 e 100 caracteres.");

        Name = name.Trim();
    }

    public override bool Equals(ValueObject? other)
    {
        if (other is not Specialization specialization)
            return false;

        return Name.Equals(specialization.Name, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Specialization specialization)
            return false;

        return Equals(specialization);
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
    }

    public override string ToString() => Name;
}
