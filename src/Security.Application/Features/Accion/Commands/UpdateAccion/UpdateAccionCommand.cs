using MediatR;
using Security.Application.Features.Accion.DTOs;

namespace Security.Application.Features.Accion.Commands.UpdateAccion;

public record UpdateAccionCommand(int Id, string Nombre, string? Descripcion, bool Activo, int UsuarioModifica) : IRequest<AccionDto>;
