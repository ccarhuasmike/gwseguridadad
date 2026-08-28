using MediatR;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Application.Features.UsuarioOpcion.Commands.SaveUsuarioPermisos;

public class SaveUsuarioPermisosCommandHandler : IRequestHandler<SaveUsuarioPermisosCommand>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IOpcionRepository _opcionRepository;
    private readonly IAccionRepository _accionRepository;
    private readonly IUsuarioOpcionRepository _usuarioOpcionRepository;
    private readonly IUsuarioAccionRepository _usuarioAccionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SaveUsuarioPermisosCommandHandler(
        IUsuarioRepository usuarioRepository,
        IOpcionRepository opcionRepository,
        IAccionRepository accionRepository,
        IUsuarioOpcionRepository usuarioOpcionRepository,
        IUsuarioAccionRepository usuarioAccionRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _opcionRepository = opcionRepository;
        _accionRepository = accionRepository;
        _usuarioOpcionRepository = usuarioOpcionRepository;
        _usuarioAccionRepository = usuarioAccionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SaveUsuarioPermisosCommand request, CancellationToken cancellationToken)
    {
        if (!await _usuarioRepository.ExistsAsync(request.IdUsuario, cancellationToken))
        {
            throw NotFoundException.For("Usuario", request.IdUsuario);
        }

        var opcionesSeleccionadas = request.Opciones.ToList();

        foreach (var seleccion in opcionesSeleccionadas)
        {
            if (!await _opcionRepository.ExistsAsync(seleccion.IdOpcion, cancellationToken))
            {
                throw NotFoundException.For("Opcion", seleccion.IdOpcion);
            }

            foreach (var idAccion in seleccion.IdAcciones.Distinct())
            {
                if (!await _accionRepository.BelongsToOpcionAsync(idAccion, seleccion.IdOpcion, cancellationToken))
                {
                    throw new BusinessException(
                        $"La acción {idAccion} no pertenece a la opción {seleccion.IdOpcion}.",
                        "ACCION_NO_PERTENECE_A_OPCION");
                }
            }
        }

        await _unitOfWork.ExecuteAsync(async (connection, transaction) =>
        {
            await _usuarioAccionRepository.RemoveAllForUsuarioAsync(request.IdUsuario, connection, transaction, cancellationToken);
            await _usuarioOpcionRepository.RemoveAllForUsuarioAsync(request.IdUsuario, connection, transaction, cancellationToken);

            foreach (var seleccion in opcionesSeleccionadas)
            {
                await _usuarioOpcionRepository.AddAsync(
                    new Domain.Entities.UsuarioOpcion
                    {
                        IdUsuario = request.IdUsuario,
                        IdOpcion = seleccion.IdOpcion,
                        UsuarioRegistro = request.UsuarioRegistro,
                        FechaRegistro = DateTime.UtcNow
                    },
                    connection,
                    transaction,
                    cancellationToken);

                foreach (var idAccion in seleccion.IdAcciones.Distinct())
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
