using MediatR;
using Security.Application.Features.UsuarioOpcion.DTOs;

namespace Security.Application.Features.UsuarioOpcion.Queries.GetUsuarioPermisos;

public record GetUsuarioPermisosQuery(int IdUsuario) : IRequest<UsuarioPermisosDto>;
