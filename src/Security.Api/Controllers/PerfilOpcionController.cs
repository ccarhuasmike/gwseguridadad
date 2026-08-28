using Microsoft.AspNetCore.Mvc;
using Security.Application.Common.Models;
using Security.Application.Features.PerfilOpcion.Commands.AssignAccionesToPerfil;
using Security.Application.Features.PerfilOpcion.Commands.AssignOpcionesToPerfil;
using Security.Application.Features.PerfilOpcion.Commands.RemoveAccionesFromPerfil;
using Security.Application.Features.PerfilOpcion.Commands.RemoveOpcionesFromPerfil;
using Security.Application.Features.PerfilOpcion.Commands.SavePerfilPermisos;
using Security.Application.Features.PerfilOpcion.DTOs;
using Security.Application.Features.PerfilOpcion.Queries.GetPerfilPermisos;

namespace Security.Api.Controllers;

/// <summary>
/// Configura los permisos (Opciones/SubOpciones/Acciones) de un Perfil,
/// persistidos en seg.PerfilOpcion y seg.PerfilAccion.
/// </summary>
[Route("api/perfiles/{idPerfil:int}/permisos")]
public class PerfilOpcionController : ApiControllerBase
{
    /// <summary>Obtiene el árbol de opciones/acciones configurado para el perfil.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PerfilPermisosDto>>> GetPermisos(int idPerfil, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetPerfilPermisosQuery(idPerfil), cancellationToken);
        return Ok(result);
    }

    /// <summary>Guarda la configuración completa de permisos del perfil en una sola operación transaccional.</summary>
    [HttpPut]
    public async Task<ActionResult<ApiResponse<object>>> SavePermisos(int idPerfil, [FromBody] GuardarPerfilPermisosDto request, CancellationToken cancellationToken)
    {
        var command = new SavePerfilPermisosCommand(idPerfil, request.Opciones, CurrentUserId);
        await Mediator.Send(command, cancellationToken);
        return OkNoContent("Permisos del perfil guardados correctamente.");
    }

    /// <summary>Asigna opciones al perfil.</summary>
    [HttpPost("opciones")]
    public async Task<ActionResult<ApiResponse<object>>> AssignOpciones(int idPerfil, [FromBody] IdsRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(new AssignOpcionesToPerfilCommand(idPerfil, request.Ids, CurrentUserId), cancellationToken);
        return OkNoContent("Opciones asignadas al perfil correctamente.");
    }

    /// <summary>Quita opciones del perfil.</summary>
    [HttpDelete("opciones")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveOpciones(int idPerfil, [FromBody] IdsRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(new RemoveOpcionesFromPerfilCommand(idPerfil, request.Ids), cancellationToken);
        return OkNoContent("Opciones removidas del perfil correctamente.");
    }

    /// <summary>Asigna acciones al perfil. Cada acción debe pertenecer a una opción ya asignada al perfil.</summary>
    [HttpPost("acciones")]
    public async Task<ActionResult<ApiResponse<object>>> AssignAcciones(int idPerfil, [FromBody] IdsRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(new AssignAccionesToPerfilCommand(idPerfil, request.Ids, CurrentUserId), cancellationToken);
        return OkNoContent("Acciones asignadas al perfil correctamente.");
    }

    /// <summary>Quita acciones del perfil.</summary>
    [HttpDelete("acciones")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveAcciones(int idPerfil, [FromBody] IdsRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(new RemoveAccionesFromPerfilCommand(idPerfil, request.Ids), cancellationToken);
        return OkNoContent("Acciones removidas del perfil correctamente.");
    }
}

public record IdsRequest(List<int> Ids);
