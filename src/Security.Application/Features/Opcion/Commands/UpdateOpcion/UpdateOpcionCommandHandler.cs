using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Opcion.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.Opcion.Commands.UpdateOpcion;

public class UpdateOpcionCommandHandler : IRequestHandler<UpdateOpcionCommand, OpcionDto>
{
    private readonly IOpcionRepository _opcionRepository;

    public UpdateOpcionCommandHandler(IOpcionRepository opcionRepository)
    {
        _opcionRepository = opcionRepository;
    }

    public async Task<OpcionDto> Handle(UpdateOpcionCommand request, CancellationToken cancellationToken)
    {
        var opcion = await _opcionRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw NotFoundException.For("Opcion", request.Id);

        if (request.IdPadre.HasValue)
        {
            if (!await _opcionRepository.ExistsAsync(request.IdPadre.Value, cancellationToken))
            {
                throw NotFoundException.For("Opcion padre", request.IdPadre.Value);
            }

            // Prevent cycles: the new parent cannot be a descendant of the option being edited.
            var ancestors = await _opcionRepository.GetAncestorIdsAsync(request.IdPadre.Value, cancellationToken);
            if (ancestors.Contains(request.Id))
            {
                throw new BusinessException("La reasignación genera un ciclo en el árbol de opciones.", "OPCION_CICLO_INVALIDO");
            }
        }

        opcion.IdPadre = request.IdPadre;
        opcion.Nombre = request.Nombre;
        opcion.Descripcion = request.Descripcion;
        opcion.Ruta = request.Ruta;
        opcion.Orden = request.Orden;
        opcion.Visible = request.Visible;
        opcion.Activo = request.Activo;
        opcion.UsuarioModifica = request.UsuarioModifica;
        opcion.FechaModifica = DateTime.UtcNow;

        await _opcionRepository.UpdateAsync(opcion, cancellationToken);

        return opcion.ToDto();
    }
}
