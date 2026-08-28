using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Accion.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.Accion.Queries.GetAccionesByOpcion;

public class GetAccionesByOpcionQueryHandler : IRequestHandler<GetAccionesByOpcionQuery, IReadOnlyList<AccionDto>>
{
    private readonly IAccionRepository _accionRepository;
    private readonly IOpcionRepository _opcionRepository;

    public GetAccionesByOpcionQueryHandler(IAccionRepository accionRepository, IOpcionRepository opcionRepository)
    {
        _accionRepository = accionRepository;
        _opcionRepository = opcionRepository;
    }

    public async Task<IReadOnlyList<AccionDto>> Handle(GetAccionesByOpcionQuery request, CancellationToken cancellationToken)
    {
        if (!await _opcionRepository.ExistsAsync(request.IdOpcion, cancellationToken))
        {
            throw NotFoundException.For("Opcion", request.IdOpcion);
        }

        var acciones = await _accionRepository.GetByOpcionAsync(request.IdOpcion, request.IncludeInactive, cancellationToken);
        return acciones.Select(a => a.ToDto()).ToList();
    }
}
