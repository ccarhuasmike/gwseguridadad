using MediatR;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Application.Features.PerfilOpcion.Commands.RemoveAccionesFromPerfil;

public class RemoveAccionesFromPerfilCommandHandler : IRequestHandler<RemoveAccionesFromPerfilCommand>
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IPerfilAccionRepository _perfilAccionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveAccionesFromPerfilCommandHandler(
        IPerfilRepository perfilRepository,
        IPerfilAccionRepository perfilAccionRepository,
        IUnitOfWork unitOfWork)
    {
        _perfilRepository = perfilRepository;
        _perfilAccionRepository = perfilAccionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveAccionesFromPerfilCommand request, CancellationToken cancellationToken)
    {
        if (!await _perfilRepository.ExistsAsync(request.IdPerfil, cancellationToken))
        {
            throw NotFoundException.For("Perfil", request.IdPerfil);
        }

        await _unitOfWork.ExecuteAsync(async (connection, transaction) =>
        {
            foreach (var idAccion in request.IdAcciones.Distinct())
            {
                await _perfilAccionRepository.RemoveAsync(request.IdPerfil, idAccion, connection, transaction, cancellationToken);
            }
        }, cancellationToken);
    }
}
