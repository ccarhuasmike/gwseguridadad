using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Opcion.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.Opcion.Queries.GetOpcionChildren;

public class GetOpcionChildrenQueryHandler : IRequestHandler<GetOpcionChildrenQuery, IReadOnlyList<OpcionDto>>
{
    private readonly IOpcionRepository _opcionRepository;

    public GetOpcionChildrenQueryHandler(IOpcionRepository opcionRepository)
    {
        _opcionRepository = opcionRepository;
    }

    public async Task<IReadOnlyList<OpcionDto>> Handle(GetOpcionChildrenQuery request, CancellationToken cancellationToken)
    {
        if (!await _opcionRepository.ExistsAsync(request.IdPadre, cancellationToken))
        {
            throw NotFoundException.For("Opcion", request.IdPadre);
        }

        var hijos = await _opcionRepository.GetChildrenAsync(request.IdPadre, cancellationToken);
        return hijos.Select(o => o.ToDto()).ToList();
    }
}
