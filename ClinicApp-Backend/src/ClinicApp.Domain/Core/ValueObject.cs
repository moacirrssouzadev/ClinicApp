namespace ClinicApp.Domain.Core;

/// <summary>
/// Classe base para Value Objects
/// </summary>
public abstract class ValueObject
{
    public abstract bool Equals(ValueObject? other);
    public abstract override bool Equals(object? obj);
    public abstract override int GetHashCode();
}
