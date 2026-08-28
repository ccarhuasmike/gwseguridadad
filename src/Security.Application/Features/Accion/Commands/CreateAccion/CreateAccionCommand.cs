using MediatR;
using Security.Application.Features.Accion.DTOs;

namespace Security.Application.Features.Accion.Commands.CreateAccion;

public record CreateAccionCommand(int IdOpcion, string Nombre, string? Descripcion, int UsuarioRegistro) : IRequest<AccionDto>;
