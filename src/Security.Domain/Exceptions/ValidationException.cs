namespace Security.Domain.Exceptions;

/// <summary>
/// Raised when input data fails validation rules. Maps to HTTP 400.
/// Aggregates one or more field-level error messages.
/// </summary>
public class ValidationException : AppExceptionBase
{
    public ValidationException(string message, IReadOnlyDictionary<string, string[]>? errors = null)
        : base("VALIDATION_ERROR", message)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
