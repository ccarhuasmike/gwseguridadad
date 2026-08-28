using MediatR;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Application.Features.Accion.Commands.DeleteAccion;

public class DeleteAccionCommandHandler : IRequestHandler<DeleteAccionCommand>
{
    private readonly IAccionRepository _accionRepository;

    public DeleteAccionCommandHandler(IAccionRepository accionRepository)
    {
        _accionRepository = accionRepository;
    }

    public async Task Handle(DeleteAccionCommand request, CancellationToken cancellationToken)
    {
        if (!await _accionRepository.ExistsAsync(request.Id, cancellationToken))
        {
            throw NotFoundException.For("Accion", request.Id);
        }

        await _accionRepository.DeactivateAsync(request.Id, request.UsuarioModifica, cancellationToken);
    }
}
