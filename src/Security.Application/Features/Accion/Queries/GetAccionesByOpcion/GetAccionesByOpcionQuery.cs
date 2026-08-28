using MediatR;
using Security.Application.Features.Accion.DTOs;

namespace Security.Application.Features.Accion.Queries.GetAccionesByOpcion;

public record GetAccionesByOpcionQuery(int IdOpcion, bool IncludeInactive = false) : IRequest<IReadOnlyList<AccionDto>>;
