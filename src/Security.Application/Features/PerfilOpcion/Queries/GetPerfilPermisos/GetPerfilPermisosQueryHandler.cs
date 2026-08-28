using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.PerfilOpcion.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.PerfilOpcion.Queries.GetPerfilPermisos;

/// <summary>Builds the full Opcion/SubOpcion/Accion tree for a Perfil, flagging the nodes currently assigned to it.</summary>
public class GetPerfilPermisosQueryHandler : IRequestHandler<GetPerfilPermisosQuery, PerfilPermisosDto>
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IOpcionRepository _opcionRepository;
    private readonly IAccionRepository _accionRepository;
    private readonly IPerfilOpcionRepository _perfilOpcionRepository;
    private readonly IPerfilAccionRepository _perfilAccionRepository;

    public GetPerfilPermisosQueryHandler(
        IPerfilRepository perfilRepository,
        IOpcionRepository opcionRepository,
        IAccionRepository accionRepository,
        IPerfilOpcionRepository perfilOpcionRepository,
        IPerfilAccionRepository perfilAccionRepository)
    {
        _perfilRepository = perfilRepository;
        _opcionRepository = opcionRepository;
        _accionRepository = accionRepository;
        _perfilOpcionRepository = perfilOpcionRepository;
        _perfilAccionRepository = perfilAccionRepository;
    }

    public async Task<PerfilPermisosDto> Handle(GetPerfilPermisosQuery request, CancellationToken cancellationToken)
    {
        if (!await _perfilRepository.ExistsAsync(request.IdPerfil, cancellationToken))
        {
            throw NotFoundException.For("Perfil", request.IdPerfil);
        }

        var opciones = await _opcionRepository.GetAllAsync(includeInactive: false, cancellationToken);
        var acciones = await _accionRepository.GetAllAsync(includeInactive: false, cancellationToken);
        var opcionesAsignadas = (await _perfilOpcionRepository.GetByPerfilAsync(request.IdPerfil, cancellationToken))
            .Select(po => po.IdOpcion)
            .ToHashSet();
        var accionesAsignadas = (await _perfilAccionRepository.GetByPerfilAsync(request.IdPerfil, cancellationToken))
            .Select(pa => pa.IdAccion)
            .ToHashSet();

        var accionesPorOpcion = acciones.ToLookup(a => a.IdOpcion);

        var nodesById = opciones.ToDictionary(o => o.Id, o => new OpcionPermisoDto
        {
            Id = o.Id,
            IdPadre = o.IdPadre,
            Nombre = o.Nombre,
            Seleccionado = opcionesAsignadas.Contains(o.Id),
            Acciones = accionesPorOpcion[o.Id]
                .Select(a => new AccionPermisoDto
                {
                    Id = a.Id,
                    Nombre = a.Nombre,
                    Seleccionado = accionesAsignadas.Contains(a.Id)
                })
                .ToList()
        });

        var roots = new List<OpcionPermisoDto>();

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

        return new PerfilPermisosDto
        {
            IdPerfil = request.IdPerfil,
            Opciones = roots
        };
    }
}
