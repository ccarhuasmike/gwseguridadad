using Microsoft.AspNetCore.Mvc;
using Security.Application.Common.Models;
using Security.Application.Features.Perfil.Commands.CreatePerfil;
using Security.Application.Features.Perfil.Commands.DeletePerfil;
using Security.Application.Features.Perfil.Commands.UpdatePerfil;
using Security.Application.Features.Perfil.DTOs;
using Security.Application.Features.Perfil.Queries.GetPerfilById;
using Security.Application.Features.Perfil.Queries.GetPerfiles;

namespace Security.Api.Controllers;

/// <summary>Administración de perfiles (seg.Perfil).</summary>
[Route("api/perfiles")]
public class PerfilesController : ApiControllerBase
{
    /// <summary>Lista los perfiles registrados.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PerfilDto>>>> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetPerfilesQuery(includeInactive), cancellationToken);
        return Ok(result);
    }

    /// <summary>Obtiene un perfil por su identificador.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PerfilDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetPerfilByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Registra un nuevo perfil.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PerfilDto>>> Create([FromBody] CreatePerfilRequest request, CancellationToken cancellationToken)
    {
        var command = new CreatePerfilCommand(request.Codigo, request.Nombre, request.Descripcion, CurrentUserId);
        var result = await Mediator.Send(command, cancellationToken);
        return Created(nameof(GetById), new { id = result.Id }, result, "Perfil creado correctamente.");
    }

    /// <summary>Actualiza un perfil existente.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<PerfilDto>>> Update(int id, [FromBody] UpdatePerfilRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdatePerfilCommand(id, request.Codigo, request.Nombre, request.Descripcion, request.Activo, CurrentUserId);
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result, "Perfil actualizado correctamente.");
    }

    /// <summary>Elimina (desactiva) un perfil.</summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeletePerfilCommand(id, CurrentUserId), cancellationToken);
        return OkNoContent("Perfil desactivado correctamente.");
    }
}

public record CreatePerfilRequest(string Codigo, string Nombre, string? Descripcion);

public record UpdatePerfilRequest(string Codigo, string Nombre, string? Descripcion, bool Activo);
