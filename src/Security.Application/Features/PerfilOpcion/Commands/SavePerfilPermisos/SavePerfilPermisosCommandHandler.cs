using MediatR;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Application.Features.PerfilOpcion.Commands.SavePerfilPermisos;

public class SavePerfilPermisosCommandHandler : IRequestHandler<SavePerfilPermisosCommand>
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IOpcionRepository _opcionRepository;
    private readonly IAccionRepository _accionRepository;
    private readonly IPerfilOpcionRepository _perfilOpcionRepository;
    private readonly IPerfilAccionRepository _perfilAccionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SavePerfilPermisosCommandHandler(
        IPerfilRepository perfilRepository,
        IOpcionRepository opcionRepository,
        IAccionRepository accionRepository,
        IPerfilOpcionRepository perfilOpcionRepository,
        IPerfilAccionRepository perfilAccionRepository,
        IUnitOfWork unitOfWork)
    {
        _perfilRepository = perfilRepository;
        _opcionRepository = opcionRepository;
        _accionRepository = accionRepository;
        _perfilOpcionRepository = perfilOpcionRepository;
        _perfilAccionRepository = perfilAccionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SavePerfilPermisosCommand request, CancellationToken cancellationToken)
    {
        if (!await _perfilRepository.ExistsAsync(request.IdPerfil, cancellationToken))
        {
            throw NotFoundException.For("Perfil", request.IdPerfil);
        }

        var opcionesSeleccionadas = request.Opciones.Distinct().ToList();

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
            // Full replace strategy: clear previous configuration, then re-insert the requested one.
            await _perfilAccionRepository.RemoveAllForPerfilAsync(request.IdPerfil, connection, transaction, cancellationToken);
            await _perfilOpcionRepository.RemoveAllForPerfilAsync(request.IdPerfil, connection, transaction, cancellationToken);

            foreach (var seleccion in opcionesSeleccionadas)
            {
                await _perfilOpcionRepository.AddAsync(
                    new Domain.Entities.PerfilOpcion
                    {
                        IdPerfil = request.IdPerfil,
                        IdOpcion = seleccion.IdOpcion,
                        UsuarioRegistro = request.UsuarioRegistro,
                        FechaRegistro = DateTime.UtcNow
                    },
                    connection,
                    transaction,
                    cancellationToken);

                foreach (var idAccion in seleccion.IdAcciones.Distinct())
                {
                    await _perfilAccionRepository.AddAsync(
                        new Domain.Entities.PerfilAccion
                        {
                            IdPerfil = request.IdPerfil,
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
