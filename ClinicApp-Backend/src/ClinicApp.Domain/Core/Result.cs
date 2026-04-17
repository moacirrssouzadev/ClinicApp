namespace ClinicApp.Domain.Core;

/// <summary>
/// Resultado base para operações de domínio
/// </summary>
public class Result
{
    public bool IsSuccess { get; protected set; }
    public string Message { get; protected set; }
    public List<string> Errors { get; protected set; }

    protected Result(bool isSuccess, string message, List<string> errors = null!)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors ?? new List<string>();
    }

    public static Result Success(string message = "Operação realizada com sucesso")
        => new(true, message);

    public static Result Failure(string message, List<string>? errors = null)
        => new(false, message, errors ?? new List<string>());

    public static Result<T> Success<T>(T data, string message = "Operação realizada com sucesso")
        => new(data, true, message);

    public static Result<T> Failure<T>(string message, List<string>? errors = null)
        => new(default!, false, message, errors ?? new List<string>());
}

/// <summary>
/// Resultado com dados genéricos
/// </summary>
public class Result<T> : Result
{
    public T Data { get; }

    public Result(T data, bool isSuccess, string message, List<string> errors = null!)
        : base(isSuccess, message, errors)
    {
        Data = data;
    }
}
