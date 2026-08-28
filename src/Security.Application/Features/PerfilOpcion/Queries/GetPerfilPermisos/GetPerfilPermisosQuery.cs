using MediatR;
using Security.Application.Features.PerfilOpcion.DTOs;

namespace Security.Application.Features.PerfilOpcion.Queries.GetPerfilPermisos;

public record GetPerfilPermisosQuery(int IdPerfil) : IRequest<PerfilPermisosDto>;
