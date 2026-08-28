using Microsoft.AspNetCore.Mvc;
using Security.Application.Common.Models;
using Security.Application.Features.Accion.DTOs;
using Security.Application.Features.Opcion.DTOs;
using Security.Application.Features.UsuarioOpcion.Commands.AssignAccionesToUsuario;
using Security.Application.Features.UsuarioOpcion.Commands.AssignOpcionesToUsuario;
using Security.Application.Features.UsuarioOpcion.Commands.RemoveAccionesFromUsuario;
using Security.Application.Features.UsuarioOpcion.Commands.RemoveOpcionesFromUsuario;
using Security.Application.Features.UsuarioOpcion.Commands.SaveUsuarioPermisos;
using Security.Application.Features.UsuarioOpcion.DTOs;
using Security.Application.Features.UsuarioOpcion.Queries.GetUsuarioAcciones;
using Security.Application.Features.UsuarioOpcion.Queries.GetUsuarioOpciones;
using Security.Application.Features.UsuarioOpcion.Queries.GetUsuarioPermisos;

namespace Security.Api.Controllers;

/// <summary>
/// Configura los permisos (Opciones/SubOpciones/Acciones) de un Usuario,
/// persistidos en seg.UsuarioOpcion y seg.UsuarioAccion.
/// </summary>
[Route("api/usuarios/{idUsuario:int}/permisos")]
public class UsuarioOpcionController : ApiControllerBase
{
    /// <summary>Obtiene el árbol de opciones/acciones configurado para el usuario.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<UsuarioPermisosDto>>> GetPermisos(int idUsuario, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetUsuarioPermisosQuery(idUsuario), cancellationToken);
        return Ok(result);
    }

    /// <summary>Obtiene únicamente las opciones configuradas para el usuario.</summary>
    [HttpGet("opciones")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OpcionDto>>>> GetOpciones(int idUsuario, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetUsuarioOpcionesQuery(idUsuario), cancellationToken);
        return Ok(result);
    }

    /// <summary>Obtiene únicamente las acciones configuradas para el usuario.</summary>
    [HttpGet("acciones")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AccionDto>>>> GetAcciones(int idUsuario, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetUsuarioAccionesQuery(idUsuario), cancellationToken);
        return Ok(result);
    }

    /// <summary>Guarda la configuración completa de permisos del usuario en una sola operación transaccional.</summary>
    [HttpPut]
    public async Task<ActionResult<ApiResponse<object>>> SavePermisos(int idUsuario, [FromBody] GuardarUsuarioPermisosDto request, CancellationToken cancellationToken)
    {
        var command = new SaveUsuarioPermisosCommand(idUsuario, request.Opciones, CurrentUserId);
        await Mediator.Send(command, cancellationToken);
        return OkNoContent("Permisos del usuario guardados correctamente.");
    }

    /// <summary>Asigna opciones al usuario.</summary>
    [HttpPost("opciones")]
    public async Task<ActionResult<ApiResponse<object>>> AssignOpciones(int idUsuario, [FromBody] IdsRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(new AssignOpcionesToUsuarioCommand(idUsuario, request.Ids, CurrentUserId), cancellationToken);
        return OkNoContent("Opciones asignadas al usuario correctamente.");
    }

    /// <summary>Quita opciones del usuario.</summary>
    [HttpDelete("opciones")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveOpciones(int idUsuario, [FromBody] IdsRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(new RemoveOpcionesFromUsuarioCommand(idUsuario, request.Ids), cancellationToken);
        return OkNoContent("Opciones removidas del usuario correctamente.");
    }

    /// <summary>Asigna acciones al usuario. Cada acción debe pertenecer a una opción ya asignada al usuario.</summary>
    [HttpPost("acciones")]
    public async Task<ActionResult<ApiResponse<object>>> AssignAcciones(int idUsuario, [FromBody] IdsRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(new AssignAccionesToUsuarioCommand(idUsuario, request.Ids, CurrentUserId), cancellationToken);
        return OkNoContent("Acciones asignadas al usuario correctamente.");
    }

    /// <summary>Quita acciones del usuario.</summary>
    [HttpDelete("acciones")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveAcciones(int idUsuario, [FromBody] IdsRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(new RemoveAccionesFromUsuarioCommand(idUsuario, request.Ids), cancellationToken);
        return OkNoContent("Acciones removidas del usuario correctamente.");
    }
}
