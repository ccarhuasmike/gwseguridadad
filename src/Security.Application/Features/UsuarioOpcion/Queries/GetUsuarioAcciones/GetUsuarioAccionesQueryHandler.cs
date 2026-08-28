using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Accion.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.UsuarioOpcion.Queries.GetUsuarioAcciones;

public class GetUsuarioAccionesQueryHandler : IRequestHandler<GetUsuarioAccionesQuery, IReadOnlyList<AccionDto>>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAccionRepository _accionRepository;
    private readonly IUsuarioAccionRepository _usuarioAccionRepository;

    public GetUsuarioAccionesQueryHandler(
        IUsuarioRepository usuarioRepository,
        IAccionRepository accionRepository,
        IUsuarioAccionRepository usuarioAccionRepository)
    {
        _usuarioRepository = usuarioRepository;
        _accionRepository = accionRepository;
        _usuarioAccionRepository = usuarioAccionRepository;
    }

    public async Task<IReadOnlyList<AccionDto>> Handle(GetUsuarioAccionesQuery request, CancellationToken cancellationToken)
    {
        if (!await _usuarioRepository.ExistsAsync(request.IdUsuario, cancellationToken))
        {
            throw NotFoundException.For("Usuario", request.IdUsuario);
        }

        var asignaciones = await _usuarioAccionRepository.GetByUsuarioAsync(request.IdUsuario, cancellationToken);
        var acciones = await _accionRepository.GetAllAsync(includeInactive: false, cancellationToken);
        var idsAsignados = asignaciones.Select(a => a.IdAccion).ToHashSet();

        return acciones.Where(a => idsAsignados.Contains(a.Id)).Select(a => a.ToDto()).ToList();
    }
}
