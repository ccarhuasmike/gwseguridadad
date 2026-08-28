namespace Security.Domain.Exceptions;

/// <summary>Raised when a requested resource does not exist. Maps to HTTP 404.</summary>
public class NotFoundException : AppExceptionBase
{
    public NotFoundException(string message, string code = "NOT_FOUND") : base(code, message)
    {
    }

    public static NotFoundException For(string entityName, object key) =>
        new($"{entityName} con identificador '{key}' no existe.", $"{entityName.ToUpperInvariant()}_NOT_FOUND");
}
