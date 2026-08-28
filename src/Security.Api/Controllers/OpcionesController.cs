using Microsoft.AspNetCore.Mvc;
using Security.Application.Common.Models;
using Security.Application.Features.Opcion.Commands.CreateOpcion;
using Security.Application.Features.Opcion.Commands.DeleteOpcion;
using Security.Application.Features.Opcion.Commands.UpdateOpcion;
using Security.Application.Features.Opcion.DTOs;
using Security.Application.Features.Opcion.Queries.GetOpcionById;
using Security.Application.Features.Opcion.Queries.GetOpcionChildren;
using Security.Application.Features.Opcion.Queries.GetOpciones;
using Security.Application.Features.Opcion.Queries.GetOpcionTree;
using Security.Application.Features.Accion.Commands.CreateAccion;
using Security.Application.Features.Accion.DTOs;
using Security.Application.Features.Accion.Queries.GetAccionesByOpcion;

namespace Security.Api.Controllers;

/// <summary>Administración de opciones recursivas del menú/sistema (seg.Opcion).</summary>
[Route("api/opciones")]
public class OpcionesController : ApiControllerBase
{
    /// <summary>Lista todas las opciones (planas, sin estructura de árbol).</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OpcionDto>>>> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetOpcionesQuery(includeInactive), cancellationToken);
        return Ok(result);
    }

    /// <summary>Obtiene el árbol completo de opciones (raíces con sus subopciones recursivas).</summary>
    [HttpGet("arbol")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OpcionTreeNodeDto>>>> GetTree([FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetOpcionTreeQuery(includeInactive), cancellationToken);
        return Ok(result);
    }

    /// <summary>Obtiene una opción por su identificador.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<OpcionDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetOpcionByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Obtiene las opciones hijas directas de una opción (subopciones).</summary>
    [HttpGet("{id:int}/hijos")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OpcionDto>>>> GetChildren(int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetOpcionChildrenQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Crea una opción raíz (IdPadre nulo) o una subopción (IdPadre informado).</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OpcionDto>>> Create([FromBody] CreateOpcionRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateOpcionCommand(
            request.IdPadre, request.Nombre, request.Descripcion, request.Ruta, request.Orden, request.Visible, CurrentUserId);
        var result = await Mediator.Send(command, cancellationToken);
        return Created(nameof(GetById), new { id = result.Id }, result, "Opción creada correctamente.");
    }

    /// <summary>Actualiza una opción existente.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<OpcionDto>>> Update(int id, [FromBody] UpdateOpcionRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateOpcionCommand(
            id, request.IdPadre, request.Nombre, request.Descripcion, request.Ruta, request.Orden, request.Visible, request.Activo, CurrentUserId);
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result, "Opción actualizada correctamente.");
    }

    /// <summary>Elimina (desactiva) una opción.</summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteOpcionCommand(id, CurrentUserId), cancellationToken);
        return OkNoContent("Opción desactivada correctamente.");
    }

    /// <summary>Lista las acciones (Crear/Editar/Consultar/Eliminar, etc.) que pertenecen a la opción.</summary>
    [HttpGet("{id:int}/acciones")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AccionDto>>>> GetAcciones(int id, [FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAccionesByOpcionQuery(id, includeInactive), cancellationToken);
        return Ok(result);
    }

    /// <summary>Registra una nueva acción para la opción.</summary>
    [HttpPost("{id:int}/acciones")]
    public async Task<ActionResult<ApiResponse<AccionDto>>> CreateAccion(int id, [FromBody] CreateAccionRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateAccionCommand(id, request.Nombre, request.Descripcion, CurrentUserId);
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result, "Acción creada correctamente.");
    }
}

public record CreateAccionRequest(string Nombre, string? Descripcion);

public record CreateOpcionRequest(int? IdPadre, string Nombre, string Descripcion, string? Ruta, byte Orden, bool Visible);

public record UpdateOpcionRequest(int? IdPadre, string Nombre, string Descripcion, string? Ruta, byte Orden, bool Visible, bool Activo);
