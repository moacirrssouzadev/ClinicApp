namespace ClinicApp.Domain.Exceptions;

/// <summary>
/// Exceção base para exceções de domínio
/// </summary>
public abstract class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

/// <summary>
/// Exceção para entidades não encontradas
/// </summary>
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object key)
        : base($"{entityName} com identificador '{key}' não foi encontrado.") { }
}

/// <summary>
/// Exceção para validações de negócio
/// </summary>
public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>
/// Exceção para dados inválidos
/// </summary>
public class InvalidDataException : DomainException
{
    public InvalidDataException(string message) : base(message) { }
}

/// <summary>
/// Exceção para recurso já existente
/// </summary>
public class ResourceAlreadyExistsException : DomainException
{
    public ResourceAlreadyExistsException(string message) : base(message) { }
}

/// <summary>
/// Exceção para operação não permitida
/// </summary>
public class OperationNotAllowedException : DomainException
{
    public OperationNotAllowedException(string message) : base(message) { }
}

/// <summary>
/// Exceção para dados corrompidos no banco de dados
/// </summary>
public class CorruptedDataException : DomainException
{
    public CorruptedDataException(string message, Exception? innerException = null) 
        : base(message) 
    {
        if (innerException != null)
            throw new InvalidOperationException(message, innerException);
    }
}
