using MediatR;
using Security.Application.Features.Opcion.DTOs;

namespace Security.Application.Features.Opcion.Queries.GetOpcionById;

public record GetOpcionByIdQuery(int Id) : IRequest<OpcionDto>;
