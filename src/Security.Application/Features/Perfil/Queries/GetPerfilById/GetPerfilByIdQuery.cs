using MediatR;
using Security.Application.Features.Perfil.DTOs;

namespace Security.Application.Features.Perfil.Queries.GetPerfilById;

public record GetPerfilByIdQuery(int Id) : IRequest<PerfilDto>;
