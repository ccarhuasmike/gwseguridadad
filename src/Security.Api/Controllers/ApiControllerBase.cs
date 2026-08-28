using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security.Application.Common.Interfaces;
using Security.Application.Common.Models;

namespace Security.Api.Controllers;

/// <summary>
/// Thin base controller: delegates all work to MediatR Commands/Queries and
/// wraps results in the standard <see cref="ApiResponse{T}"/> envelope. No
/// business logic or data access code belongs in a Controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;
    private ICurrentUserService? _currentUserService;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected ICurrentUserService CurrentUserService =>
        _currentUserService ??= HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

    /// <summary>Id of the caller for audit columns; falls back to 0 (system) until authentication is wired in.</summary>
    protected int CurrentUserId => CurrentUserService.UserId ?? 0;

    protected ActionResult<ApiResponse<T>> Ok<T>(T data, string message = "Operación realizada correctamente.") =>
        base.Ok(ApiResponse<T>.Ok(data, message, HttpContext.TraceIdentifier));

    protected ActionResult<ApiResponse<object>> OkNoContent(string message = "Operación realizada correctamente.") =>
        base.Ok(ApiResponse<object>.Ok(null, message, HttpContext.TraceIdentifier));

    protected ActionResult<ApiResponse<T>> Created<T>(string actionName, object routeValues, T data, string message = "Recurso creado correctamente.") =>
        base.CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(data, message, HttpContext.TraceIdentifier));
}
