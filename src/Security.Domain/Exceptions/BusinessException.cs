namespace Security.Domain.Exceptions;

/// <summary>Raised when a business/domain rule is violated. Maps to HTTP 422.</summary>
public class BusinessException : AppExceptionBase
{
    public BusinessException(string message, string code = "BUSINESS_RULE_VIOLATION") : base(code, message)
    {
    }
}
