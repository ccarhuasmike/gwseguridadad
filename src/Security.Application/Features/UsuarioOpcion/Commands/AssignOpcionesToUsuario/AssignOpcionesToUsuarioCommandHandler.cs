using MediatR;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Application.Features.UsuarioOpcion.Commands.AssignOpcionesToUsuario;

public class AssignOpcionesToUsuarioCommandHandler : IRequestHandler<AssignOpcionesToUsuarioCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IOpcionRepository _opcionRepository;
    private readonly IUsuarioOpcionRepository _usuarioOpcionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignOpcionesToUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository,
        IOpcionRepository opcionRepository,
        IUsuarioOpcionRepository usuarioOpcionRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _opcionRepository = opcionRepository;
        _usuarioOpcionRepository = usuarioOpcionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AssignOpcionesToUsuarioCommand request, CancellationToken cancellationToken)
    {
        if (!await _usuarioRepository.ExistsAsync(request.IdUsuario, cancellationToken))
        {
            throw NotFoundException.For("Usuario", request.IdUsuario);
        }

        foreach (var idOpcion in request.IdOpciones.Distinct())
        {
            if (!await _opcionRepository.ExistsAsync(idOpcion, cancellationToken))
            {
                throw NotFoundException.For("Opcion", idOpcion);
            }
        }

        await _unitOfWork.ExecuteAsync(async (connection, transaction) =>
        {
            foreach (var idOpcion in request.IdOpciones.Distinct())
            {
                if (!await _usuarioOpcionRepository.ExistsAsync(request.IdUsuario, idOpcion, cancellationToken))
                {
                    await _usuarioOpcionRepository.AddAsync(
                        new Domain.Entities.UsuarioOpcion
                        {
                            IdUsuario = request.IdUsuario,
                            IdOpcion = idOpcion,
                            UsuarioRegistro = request.UsuarioRegistro,
                            FechaRegistro = DateTime.UtcNow
                        },
                        connection,
                        transaction,
                        cancellationToken);
                }
            }
        }, cancellationToken);
    }
}
