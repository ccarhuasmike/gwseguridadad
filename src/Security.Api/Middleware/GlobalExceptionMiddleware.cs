using System.Net;
using Security.Application.Common.Interfaces;
using Security.Application.Common.Models;
using Security.Domain.Exceptions;
using ValidationException = Security.Domain.Exceptions.ValidationException;

namespace Security.Api.Middleware;

/// <summary>
/// Centralized exception handling middleware. Every request goes through this
/// middleware so individual Controllers never need try/catch blocks: each
/// custom exception type is mapped to the appropriate HTTP status code and a
/// standard <see cref="ApiResponse{T}"/> envelope, and every unhandled error
/// is logged with full diagnostic context (timestamp, trace id, endpoint,
/// HTTP method, user and stack trace).
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception, currentUserService);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, ICurrentUserService currentUserService)
    {
        var traceId = context.TraceIdentifier;
        var (statusCode, errors, message) = MapException(exception);

        _logger.LogError(
            exception,
            "Error no controlado. Timestamp: {Timestamp}, TraceId: {TraceId}, Endpoint: {Endpoint}, Method: {Method}, User: {User}, Message: {ExceptionMessage}",
            DateTime.UtcNow,
            traceId,
            context.Request.Path,
            context.Request.Method,
            currentUserService.UserName ?? currentUserService.UserId?.ToString() ?? "anónimo",
            exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(message, errors, traceId);

        await context.Response.WriteAsJsonAsync(response);
    }

    private static (HttpStatusCode StatusCode, List<ApiError> Errors, string Message) MapException(Exception exception) => exception switch
    {
        ValidationException validationException => (
            HttpStatusCode.BadRequest,
            validationException.Errors
                .SelectMany(kvp => kvp.Value.Select(msg => new ApiError(kvp.Key, msg)))
                .DefaultIfEmpty(new ApiError(validationException.Code, validationException.Message))
                .ToList(),
            "Uno o más campos no son válidos."),

        NotFoundException notFoundException => (
            HttpStatusCode.NotFound,
            new List<ApiError> { new(notFoundException.Code, notFoundException.Message) },
            "El recurso solicitado no existe."),

        UnauthorizedException unauthorizedException => (
            HttpStatusCode.Unauthorized,
            new List<ApiError> { new(unauthorizedException.Code, unauthorizedException.Message) },
            "No se encuentra autenticado."),

        ForbiddenException forbiddenException => (
            HttpStatusCode.Forbidden,
            new List<ApiError> { new(forbiddenException.Code, forbiddenException.Message) },
            "No tiene permisos para realizar esta operación."),

        BusinessException businessException => (
            HttpStatusCode.UnprocessableEntity,
            new List<ApiError> { new(businessException.Code, businessException.Message) },
            "Se violó una regla de negocio."),

        DatabaseException databaseException => (
            HttpStatusCode.InternalServerError,
            new List<ApiError> { new(databaseException.Code, "Ocurrió un error al acceder a la base de datos.") },
            "Se produjo un error al procesar la solicitud."),

        _ => (
            HttpStatusCode.InternalServerError,
            new List<ApiError> { new("INTERNAL_SERVER_ERROR", "Ocurrió un error inesperado.") },
            "Se produjo un error al procesar la solicitud.")
    };
}
