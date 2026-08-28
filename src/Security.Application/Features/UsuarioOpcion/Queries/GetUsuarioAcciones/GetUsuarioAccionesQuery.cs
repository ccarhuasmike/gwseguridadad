using MediatR;
using Security.Application.Features.Accion.DTOs;

namespace Security.Application.Features.UsuarioOpcion.Queries.GetUsuarioAcciones;

public record GetUsuarioAccionesQuery(int IdUsuario) : IRequest<IReadOnlyList<AccionDto>>;
