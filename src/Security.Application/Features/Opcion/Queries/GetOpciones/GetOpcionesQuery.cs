using MediatR;
using Security.Application.Features.Opcion.DTOs;

namespace Security.Application.Features.Opcion.Queries.GetOpciones;

public record GetOpcionesQuery(bool IncludeInactive = false) : IRequest<IReadOnlyList<OpcionDto>>;
