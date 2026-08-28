using MediatR;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Application.Features.UsuarioOpcion.Commands.AssignAccionesToUsuario;

/// <summary>
/// Assigns Acciones directly to a Usuario. An Accion can only be assigned if
/// its parent Opcion has already been assigned to the Usuario
/// (seg.UsuarioOpcion).
/// </summary>
public class AssignAccionesToUsuarioCommandHandler : IRequestHandler<AssignAccionesToUsuarioCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAccionRepository _accionRepository;
    private readonly IUsuarioOpcionRepository _usuarioOpcionRepository;
    private readonly IUsuarioAccionRepository _usuarioAccionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignAccionesToUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository,
        IAccionRepository accionRepository,
        IUsuarioOpcionRepository usuarioOpcionRepository,
        IUsuarioAccionRepository usuarioAccionRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _accionRepository = accionRepository;
        _usuarioOpcionRepository = usuarioOpcionRepository;
        _usuarioAccionRepository = usuarioAccionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AssignAccionesToUsuarioCommand request, CancellationToken cancellationToken)
    {
        if (!await _usuarioRepository.ExistsAsync(request.IdUsuario, cancellationToken))
        {
            throw NotFoundException.For("Usuario", request.IdUsuario);
        }

        var idAcciones = request.IdAcciones.Distinct().ToList();

        foreach (var idAccion in idAcciones)
        {
            var accion = await _accionRepository.GetByIdAsync(idAccion, cancellationToken)
                ?? throw NotFoundException.For("Accion", idAccion);

            if (!await _usuarioOpcionRepository.ExistsAsync(request.IdUsuario, accion.IdOpcion, cancellationToken))
            {
                throw new BusinessException(
                    $"La acción '{accion.Nombre}' no puede asignarse porque su opción no está asignada al usuario.",
                    "ACCION_OPCION_NO_ASIGNADA");
            }
        }

        await _unitOfWork.ExecuteAsync(async (connection, transaction) =>
        {
            foreach (var idAccion in idAcciones)
            {
                if (!await _usuarioAccionRepository.ExistsAsync(request.IdUsuario, idAccion, cancellationToken))
                {
                    await _usuarioAccionRepository.AddAsync(
                        new Domain.Entities.UsuarioAccion
                        {
                            IdUsuario = request.IdUsuario,
                            IdAccion = idAccion,
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
