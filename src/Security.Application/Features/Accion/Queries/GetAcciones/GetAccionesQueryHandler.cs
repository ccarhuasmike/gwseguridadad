using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Accion.DTOs;

namespace Security.Application.Features.Accion.Queries.GetAcciones;

public class GetAccionesQueryHandler : IRequestHandler<GetAccionesQuery, IReadOnlyList<AccionDto>>
{
    private readonly IAccionRepository _accionRepository;

    public GetAccionesQueryHandler(IAccionRepository accionRepository)
    {
        _accionRepository = accionRepository;
    }

    public async Task<IReadOnlyList<AccionDto>> Handle(GetAccionesQuery request, CancellationToken cancellationToken)
    {
        var acciones = await _accionRepository.GetAllAsync(request.IncludeInactive, cancellationToken);
        return acciones.Select(a => a.ToDto()).ToList();
    }
}
