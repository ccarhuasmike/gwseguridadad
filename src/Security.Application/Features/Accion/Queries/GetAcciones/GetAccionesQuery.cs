using MediatR;
using Security.Application.Features.Accion.DTOs;

namespace Security.Application.Features.Accion.Queries.GetAcciones;

public record GetAccionesQuery(bool IncludeInactive = false) : IRequest<IReadOnlyList<AccionDto>>;
