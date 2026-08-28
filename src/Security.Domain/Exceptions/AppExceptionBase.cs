namespace Security.Domain.Exceptions;

/// <summary>
/// Base type for every custom exception raised by the domain/application
/// layers. Carries an error <see cref="Code"/> used by clients to react
/// programmatically to specific failures.
/// </summary>
public abstract class AppExceptionBase : Exception
{
    protected AppExceptionBase(string code, string message) : base(message)
    {
        Code = code;
    }

    protected AppExceptionBase(string code, string message, Exception innerException)
        : base(message, innerException)
    {
        Code = code;
    }

    public string Code { get; }
}
