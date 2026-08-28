using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Perfil.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.Perfil.Commands.UpdatePerfil;

public class UpdatePerfilCommandHandler : IRequestHandler<UpdatePerfilCommand, PerfilDto>
{
    private readonly IPerfilRepository _perfilRepository;

    public UpdatePerfilCommandHandler(IPerfilRepository perfilRepository)
    {
        _perfilRepository = perfilRepository;
    }

    public async Task<PerfilDto> Handle(UpdatePerfilCommand request, CancellationToken cancellationToken)
    {
        var perfil = await _perfilRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw NotFoundException.For("Perfil", request.Id);

        if (await _perfilRepository.ExistsByCodigoAsync(request.Codigo, excludeId: request.Id, cancellationToken))
        {
            throw new BusinessException($"Ya existe un perfil con el código '{request.Codigo}'.", "PERFIL_CODIGO_DUPLICADO");
        }

        perfil.Codigo = request.Codigo;
        perfil.Nombre = request.Nombre;
        perfil.Descripcion = request.Descripcion;
        perfil.Activo = request.Activo;
        perfil.UsuarioModifica = request.UsuarioModifica;
        perfil.FechaModifica = DateTime.UtcNow;

        await _perfilRepository.UpdateAsync(perfil, cancellationToken);

        return perfil.ToDto();
    }
}
