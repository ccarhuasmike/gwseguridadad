using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Opcion.DTOs;

namespace Security.Application.Features.Opcion.Queries.GetOpciones;

public class GetOpcionesQueryHandler : IRequestHandler<GetOpcionesQuery, IReadOnlyList<OpcionDto>>
{
    private readonly IOpcionRepository _opcionRepository;

    public GetOpcionesQueryHandler(IOpcionRepository opcionRepository)
    {
        _opcionRepository = opcionRepository;
    }

    public async Task<IReadOnlyList<OpcionDto>> Handle(GetOpcionesQuery request, CancellationToken cancellationToken)
    {
        var opciones = await _opcionRepository.GetAllAsync(request.IncludeInactive, cancellationToken);
        return opciones.Select(o => o.ToDto()).ToList();
    }
}
