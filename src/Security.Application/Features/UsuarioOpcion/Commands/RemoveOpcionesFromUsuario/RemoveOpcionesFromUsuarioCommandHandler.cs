using MediatR;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Application.Features.UsuarioOpcion.Commands.RemoveOpcionesFromUsuario;

public class RemoveOpcionesFromUsuarioCommandHandler : IRequestHandler<RemoveOpcionesFromUsuarioCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUsuarioOpcionRepository _usuarioOpcionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveOpcionesFromUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository,
        IUsuarioOpcionRepository usuarioOpcionRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _usuarioOpcionRepository = usuarioOpcionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveOpcionesFromUsuarioCommand request, CancellationToken cancellationToken)
    {
        if (!await _usuarioRepository.ExistsAsync(request.IdUsuario, cancellationToken))
        {
            throw NotFoundException.For("Usuario", request.IdUsuario);
        }

        await _unitOfWork.ExecuteAsync(async (connection, transaction) =>
        {
            foreach (var idOpcion in request.IdOpciones.Distinct())
            {
                await _usuarioOpcionRepository.RemoveAsync(request.IdUsuario, idOpcion, connection, transaction, cancellationToken);
            }
        }, cancellationToken);
    }
}
