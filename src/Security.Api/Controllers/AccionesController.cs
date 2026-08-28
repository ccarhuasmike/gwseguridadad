using Microsoft.AspNetCore.Mvc;
using Security.Application.Common.Models;
using Security.Application.Features.Accion.Commands.DeleteAccion;
using Security.Application.Features.Accion.Commands.UpdateAccion;
using Security.Application.Features.Accion.DTOs;
using Security.Application.Features.Accion.Queries.GetAcciones;

namespace Security.Api.Controllers;

/// <summary>Administración de acciones asociadas a las opciones (seg.Accion).</summary>
[Route("api/acciones")]
public class AccionesController : ApiControllerBase
{
    /// <summary>Lista todas las acciones registradas en el sistema.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AccionDto>>>> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAccionesQuery(includeInactive), cancellationToken);
        return Ok(result);
    }

    /// <summary>Actualiza una acción existente.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<AccionDto>>> Update(int id, [FromBody] UpdateAccionRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateAccionCommand(id, request.Nombre, request.Descripcion, request.Activo, CurrentUserId);
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result, "Acción actualizada correctamente.");
    }

    /// <summary>Elimina (desactiva) una acción.</summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteAccionCommand(id, CurrentUserId), cancellationToken);
        return OkNoContent("Acción desactivada correctamente.");
    }
}

public record UpdateAccionRequest(string Nombre, string? Descripcion, bool Activo);
