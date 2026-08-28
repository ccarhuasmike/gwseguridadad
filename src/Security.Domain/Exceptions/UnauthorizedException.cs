namespace Security.Domain.Exceptions;

/// <summary>Raised when the caller could not be authenticated. Maps to HTTP 401.</summary>
public class UnauthorizedException : AppExceptionBase
{
    public UnauthorizedException(string message = "No se encuentra autenticado.") : base("UNAUTHORIZED", message)
    {
    }
}
