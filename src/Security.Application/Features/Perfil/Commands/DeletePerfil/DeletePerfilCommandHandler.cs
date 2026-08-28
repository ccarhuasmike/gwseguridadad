using MediatR;
using Security.Application.Common.Interfaces;
using Security.Domain.Exceptions;

namespace Security.Application.Features.Perfil.Commands.DeletePerfil;

/// <summary>Deactivates (soft-deletes) a Perfil; records are never physically removed.</summary>
public class DeletePerfilCommandHandler : IRequestHandler<DeletePerfilCommand>
{
    private readonly IPerfilRepository _perfilRepository;

    public DeletePerfilCommandHandler(IPerfilRepository perfilRepository)
    {
        _perfilRepository = perfilRepository;
    }

    public async Task Handle(DeletePerfilCommand request, CancellationToken cancellationToken)
    {
        if (!await _perfilRepository.ExistsAsync(request.Id, cancellationToken))
        {
            throw NotFoundException.For("Perfil", request.Id);
        }

        await _perfilRepository.DeactivateAsync(request.Id, request.UsuarioModifica, cancellationToken);
    }
}
