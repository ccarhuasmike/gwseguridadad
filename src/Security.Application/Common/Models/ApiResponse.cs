namespace Security.Application.Common.Models;

/// <summary>Standard, consistent envelope returned by every API endpoint.</summary>
public class ApiResponse<T>
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public T? Data { get; init; }

    public List<ApiError> Errors { get; init; } = new();

    public string? TraceId { get; init; }

    public static ApiResponse<T> Ok(T? data, string message = "Operación realizada correctamente.", string? traceId = null) =>
        new() { Success = true, Message = message, Data = data, TraceId = traceId };

    public static ApiResponse<T> Fail(string message, IEnumerable<ApiError>? errors = null, string? traceId = null) =>
        new()
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors?.ToList() ?? new List<ApiError>(),
            TraceId = traceId
        };
}

/// <summary>Individual error item returned within an <see cref="ApiResponse{T}"/>.</summary>
public class ApiError
{
    public ApiError()
    {
    }

    public ApiError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}
