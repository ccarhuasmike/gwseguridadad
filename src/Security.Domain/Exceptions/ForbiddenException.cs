namespace Security.Domain.Exceptions;

/// <summary>Raised when the caller lacks permission for the operation. Maps to HTTP 403.</summary>
public class ForbiddenException : AppExceptionBase
{
    public ForbiddenException(string message = "No tiene permisos para realizar esta operación.") : base("FORBIDDEN", message)
    {
    }
}
