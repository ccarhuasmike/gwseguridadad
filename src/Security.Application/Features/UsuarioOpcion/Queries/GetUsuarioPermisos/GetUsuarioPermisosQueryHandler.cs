using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.UsuarioOpcion.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.UsuarioOpcion.Queries.GetUsuarioPermisos;

/// <summary>Builds the full Opcion/SubOpcion/Accion tree for a Usuario, flagging the nodes currently assigned to it.</summary>
public class GetUsuarioPermisosQueryHandler : IRequestHandler<GetUsuarioPermisosQuery, UsuarioPermisosDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IOpcionRepository _opcionRepository;
    private readonly IAccionRepository _accionRepository;
    private readonly IUsuarioOpcionRepository _usuarioOpcionRepository;
    private readonly IUsuarioAccionRepository _usuarioAccionRepository;

    public GetUsuarioPermisosQueryHandler(
        IUsuarioRepository usuarioRepository,
        IOpcionRepository opcionRepository,
        IAccionRepository accionRepository,
        IUsuarioOpcionRepository usuarioOpcionRepository,
        IUsuarioAccionRepository usuarioAccionRepository)
    {
        _usuarioRepository = usuarioRepository;
        _opcionRepository = opcionRepository;
        _accionRepository = accionRepository;
        _usuarioOpcionRepository = usuarioOpcionRepository;
        _usuarioAccionRepository = usuarioAccionRepository;
    }

    public async Task<UsuarioPermisosDto> Handle(GetUsuarioPermisosQuery request, CancellationToken cancellationToken)
    {
        if (!await _usuarioRepository.ExistsAsync(request.IdUsuario, cancellationToken))
        {
            throw NotFoundException.For("Usuario", request.IdUsuario);
        }

        var opciones = await _opcionRepository.GetAllAsync(includeInactive: false, cancellationToken);
        var acciones = await _accionRepository.GetAllAsync(includeInactive: false, cancellationToken);
        var opcionesAsignadas = (await _usuarioOpcionRepository.GetByUsuarioAsync(request.IdUsuario, cancellationToken))
            .Select(uo => uo.IdOpcion)
            .ToHashSet();
        var accionesAsignadas = (await _usuarioAccionRepository.GetByUsuarioAsync(request.IdUsuario, cancellationToken))
            .Select(ua => ua.IdAccion)
            .ToHashSet();

        var accionesPorOpcion = acciones.ToLookup(a => a.IdOpcion);

        var nodesById = opciones.ToDictionary(o => o.Id, o => new OpcionPermisoUsuarioDto
        {
            Id = o.Id,
            IdPadre = o.IdPadre,
            Nombre = o.Nombre,
            Seleccionado = opcionesAsignadas.Contains(o.Id),
            Acciones = accionesPorOpcion[o.Id]
                .Select(a => new AccionPermisoUsuarioDto
                {
                    Id = a.Id,
                    Nombre = a.Nombre,
                    Seleccionado = accionesAsignadas.Contains(a.Id)
                })
                .ToList()
        });

        var roots = new List<OpcionPermisoUsuarioDto>();

        foreach (var node in nodesById.Values)
        {
            if (node.IdPadre.HasValue && nodesById.TryGetValue(node.IdPadre.Value, out var parent))
            {
                parent.Hijos.Add(node);
            }
            else
            {
                roots.Add(node);
            }
        }

        return new UsuarioPermisosDto
        {
            IdUsuario = request.IdUsuario,
            Opciones = roots
        };
    }
}
