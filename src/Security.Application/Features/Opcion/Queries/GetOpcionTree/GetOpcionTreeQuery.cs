using MediatR;
using Security.Application.Features.Opcion.DTOs;

namespace Security.Application.Features.Opcion.Queries.GetOpcionTree;

/// <summary>Returns the full recursive tree of options starting at root nodes (IdPadre == null).</summary>
public record GetOpcionTreeQuery(bool IncludeInactive = false) : IRequest<IReadOnlyList<OpcionTreeNodeDto>>;
