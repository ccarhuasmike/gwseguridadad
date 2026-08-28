using MediatR;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Application.Features.Opcion.Commands.DeleteOpcion;

/// <summary>Deactivates (soft-deletes) an Opcion after ensuring it has no active children/acciones depending on it.</summary>
public class DeleteOpcionCommandHandler : IRequestHandler<DeleteOpcionCommand>
{
    private readonly IOpcionRepository _opcionRepository;

    public DeleteOpcionCommandHandler(IOpcionRepository opcionRepository)
    {
        _opcionRepository = opcionRepository;
    }

    public async Task Handle(DeleteOpcionCommand request, CancellationToken cancellationToken)
    {
        if (!await _opcionRepository.ExistsAsync(request.Id, cancellationToken))
        {
            throw NotFoundException.For("Opcion", request.Id);
        }

        if (await _opcionRepository.HasChildrenAsync(request.Id, cancellationToken))
        {
            throw new BusinessException("No se puede eliminar la opción porque tiene subopciones activas.", "OPCION_TIENE_HIJOS");
        }

        if (await _opcionRepository.HasAccionesAsync(request.Id, cancellationToken))
        {
            throw new BusinessException("No se puede eliminar la opción porque tiene acciones activas asociadas.", "OPCION_TIENE_ACCIONES");
        }

        await _opcionRepository.DeactivateAsync(request.Id, request.UsuarioModifica, cancellationToken);
    }
}
