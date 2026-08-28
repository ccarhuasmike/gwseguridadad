using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Opcion.DTOs;

namespace Security.Application.Features.Opcion.Queries.GetOpcionTree;

public class GetOpcionTreeQueryHandler : IRequestHandler<GetOpcionTreeQuery, IReadOnlyList<OpcionTreeNodeDto>>
{
    private readonly IOpcionRepository _opcionRepository;

    public GetOpcionTreeQueryHandler(IOpcionRepository opcionRepository)
    {
        _opcionRepository = opcionRepository;
    }

    public async Task<IReadOnlyList<OpcionTreeNodeDto>> Handle(GetOpcionTreeQuery request, CancellationToken cancellationToken)
    {
        var opciones = await _opcionRepository.GetAllAsync(request.IncludeInactive, cancellationToken);

        var nodesById = opciones.ToDictionary(
            o => o.Id,
            o => new OpcionTreeNodeDto
            {
                Id = o.Id,
                IdPadre = o.IdPadre,
                Nombre = o.Nombre,
                Descripcion = o.Descripcion,
                Ruta = o.Ruta,
                Orden = o.Orden,
                Visible = o.Visible,
                Activo = o.Activo
            });

        var roots = new List<OpcionTreeNodeDto>();

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

        return OrderTree(roots);
    }

    private static List<OpcionTreeNodeDto> OrderTree(List<OpcionTreeNodeDto> nodes)
    {
        foreach (var node in nodes)
        {
            node.Hijos = OrderTree(node.Hijos);
        }

        return nodes.OrderBy(n => n.Orden).ThenBy(n => n.Nombre).ToList();
    }
}
