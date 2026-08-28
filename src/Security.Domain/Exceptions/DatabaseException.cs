namespace Security.Domain.Exceptions;

/// <summary>Raised when a persistence operation fails unexpectedly. Maps to HTTP 500.</summary>
public class DatabaseException : AppExceptionBase
{
    public DatabaseException(string message, Exception innerException)
        : base("DATABASE_ERROR", message, innerException)
    {
    }
}
