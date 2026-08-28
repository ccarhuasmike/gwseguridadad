using MediatR;
using Security.Application.Features.Opcion.DTOs;

namespace Security.Application.Features.Opcion.Queries.GetOpcionChildren;

public record GetOpcionChildrenQuery(int IdPadre) : IRequest<IReadOnlyList<OpcionDto>>;
