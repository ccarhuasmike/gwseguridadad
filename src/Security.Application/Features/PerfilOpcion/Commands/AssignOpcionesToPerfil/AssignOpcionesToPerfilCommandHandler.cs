using System.Data;
using MediatR;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Application.Features.PerfilOpcion.Commands.AssignOpcionesToPerfil;

public class AssignOpcionesToPerfilCommandHandler : IRequestHandler<AssignOpcionesToPerfilCommand>
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IOpcionRepository _opcionRepository;
    private readonly IPerfilOpcionRepository _perfilOpcionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignOpcionesToPerfilCommandHandler(
        IPerfilRepository perfilRepository,
        IOpcionRepository opcionRepository,
        IPerfilOpcionRepository perfilOpcionRepository,
        IUnitOfWork unitOfWork)
    {
        _perfilRepository = perfilRepository;
        _opcionRepository = opcionRepository;
        _perfilOpcionRepository = perfilOpcionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AssignOpcionesToPerfilCommand request, CancellationToken cancellationToken)
    {
        if (!await _perfilRepository.ExistsAsync(request.IdPerfil, cancellationToken))
        {
            throw NotFoundException.For("Perfil", request.IdPerfil);
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
                if (!await _perfilOpcionRepository.ExistsAsync(request.IdPerfil, idOpcion, cancellationToken))
                {
                    await _perfilOpcionRepository.AddAsync(
                        new Domain.Entities.PerfilOpcion
                        {
                            IdPerfil = request.IdPerfil,
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
