using MediatR;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Application.Features.UsuarioOpcion.Commands.RemoveAccionesFromUsuario;

public class RemoveAccionesFromUsuarioCommandHandler : IRequestHandler<RemoveAccionesFromUsuarioCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUsuarioAccionRepository _usuarioAccionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveAccionesFromUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository,
        IUsuarioAccionRepository usuarioAccionRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _usuarioAccionRepository = usuarioAccionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveAccionesFromUsuarioCommand request, CancellationToken cancellationToken)
    {
        if (!await _usuarioRepository.ExistsAsync(request.IdUsuario, cancellationToken))
        {
            throw NotFoundException.For("Usuario", request.IdUsuario);
        }

        await _unitOfWork.ExecuteAsync(async (connection, transaction) =>
        {
            foreach (var idAccion in request.IdAcciones.Distinct())
            {
                await _usuarioAccionRepository.RemoveAsync(request.IdUsuario, idAccion, connection, transaction, cancellationToken);
            }
        }, cancellationToken);
    }
}
