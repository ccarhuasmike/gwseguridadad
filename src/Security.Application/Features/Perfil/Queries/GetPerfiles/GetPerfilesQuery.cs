using MediatR;
using Security.Application.Features.Perfil.DTOs;

namespace Security.Application.Features.Perfil.Queries.GetPerfiles;

public record GetPerfilesQuery(bool IncludeInactive = false) : IRequest<IReadOnlyList<PerfilDto>>;
