using MediatR;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Application.Features.PerfilOpcion.Commands.AssignAccionesToPerfil;

/// <summary>
/// Assigns Acciones to a Perfil. An Accion can only be assigned if its parent
/// Opcion has already been assigned to the Perfil (seg.PerfilOpcion), as
/// required by business rule "una acción siempre pertenece a una opción".
/// </summary>
public class AssignAccionesToPerfilCommandHandler : IRequestHandler<AssignAccionesToPerfilCommand>
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IAccionRepository _accionRepository;
    private readonly IPerfilOpcionRepository _perfilOpcionRepository;
    private readonly IPerfilAccionRepository _perfilAccionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignAccionesToPerfilCommandHandler(
        IPerfilRepository perfilRepository,
        IAccionRepository accionRepository,
        IPerfilOpcionRepository perfilOpcionRepository,
        IPerfilAccionRepository perfilAccionRepository,
        IUnitOfWork unitOfWork)
    {
        _perfilRepository = perfilRepository;
        _accionRepository = accionRepository;
        _perfilOpcionRepository = perfilOpcionRepository;
        _perfilAccionRepository = perfilAccionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AssignAccionesToPerfilCommand request, CancellationToken cancellationToken)
    {
        if (!await _perfilRepository.ExistsAsync(request.IdPerfil, cancellationToken))
        {
            throw NotFoundException.For("Perfil", request.IdPerfil);
        }

        var idAcciones = request.IdAcciones.Distinct().ToList();

        foreach (var idAccion in idAcciones)
        {
            var accion = await _accionRepository.GetByIdAsync(idAccion, cancellationToken)
                ?? throw NotFoundException.For("Accion", idAccion);

            if (!await _perfilOpcionRepository.ExistsAsync(request.IdPerfil, accion.IdOpcion, cancellationToken))
            {
                throw new BusinessException(
                    $"La acción '{accion.Nombre}' no puede asignarse porque su opción no está asignada al perfil.",
                    "ACCION_OPCION_NO_ASIGNADA");
            }
        }

        await _unitOfWork.ExecuteAsync(async (connection, transaction) =>
        {
            foreach (var idAccion in idAcciones)
            {
                if (!await _perfilAccionRepository.ExistsAsync(request.IdPerfil, idAccion, cancellationToken))
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
