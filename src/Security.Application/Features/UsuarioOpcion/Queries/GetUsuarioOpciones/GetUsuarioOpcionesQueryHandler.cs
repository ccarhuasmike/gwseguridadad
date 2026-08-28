using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Opcion.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.UsuarioOpcion.Queries.GetUsuarioOpciones;

public class GetUsuarioOpcionesQueryHandler : IRequestHandler<GetUsuarioOpcionesQuery, IReadOnlyList<OpcionDto>>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IOpcionRepository _opcionRepository;
    private readonly IUsuarioOpcionRepository _usuarioOpcionRepository;

    public GetUsuarioOpcionesQueryHandler(
        IUsuarioRepository usuarioRepository,
        IOpcionRepository opcionRepository,
        IUsuarioOpcionRepository usuarioOpcionRepository)
    {
        _usuarioRepository = usuarioRepository;
        _opcionRepository = opcionRepository;
        _usuarioOpcionRepository = usuarioOpcionRepository;
    }

    public async Task<IReadOnlyList<OpcionDto>> Handle(GetUsuarioOpcionesQuery request, CancellationToken cancellationToken)
    {
        if (!await _usuarioRepository.ExistsAsync(request.IdUsuario, cancellationToken))
        {
            throw NotFoundException.For("Usuario", request.IdUsuario);
        }

        var asignaciones = await _usuarioOpcionRepository.GetByUsuarioAsync(request.IdUsuario, cancellationToken);
        var opciones = await _opcionRepository.GetAllAsync(includeInactive: false, cancellationToken);
        var idsAsignados = asignaciones.Select(a => a.IdOpcion).ToHashSet();

        return opciones.Where(o => idsAsignados.Contains(o.Id)).Select(o => o.ToDto()).ToList();
    }
}
