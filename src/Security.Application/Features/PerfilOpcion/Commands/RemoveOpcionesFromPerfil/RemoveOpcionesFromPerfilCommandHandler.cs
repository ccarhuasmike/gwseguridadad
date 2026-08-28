using MediatR;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Application.Features.PerfilOpcion.Commands.RemoveOpcionesFromPerfil;

public class RemoveOpcionesFromPerfilCommandHandler : IRequestHandler<RemoveOpcionesFromPerfilCommand>
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IPerfilOpcionRepository _perfilOpcionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveOpcionesFromPerfilCommandHandler(
        IPerfilRepository perfilRepository,
        IPerfilOpcionRepository perfilOpcionRepository,
        IUnitOfWork unitOfWork)
    {
        _perfilRepository = perfilRepository;
        _perfilOpcionRepository = perfilOpcionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveOpcionesFromPerfilCommand request, CancellationToken cancellationToken)
    {
        if (!await _perfilRepository.ExistsAsync(request.IdPerfil, cancellationToken))
        {
            throw NotFoundException.For("Perfil", request.IdPerfil);
        }

        await _unitOfWork.ExecuteAsync(async (connection, transaction) =>
        {
            foreach (var idOpcion in request.IdOpciones.Distinct())
            {
                await _perfilOpcionRepository.RemoveAsync(request.IdPerfil, idOpcion, connection, transaction, cancellationToken);
            }
        }, cancellationToken);
    }
}
