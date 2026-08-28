using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Perfil.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.Perfil.Commands.CreatePerfil;

public class CreatePerfilCommandHandler : IRequestHandler<CreatePerfilCommand, PerfilDto>
{
    private readonly IPerfilRepository _perfilRepository;

    public CreatePerfilCommandHandler(IPerfilRepository perfilRepository)
    {
        _perfilRepository = perfilRepository;
    }

    public async Task<PerfilDto> Handle(CreatePerfilCommand request, CancellationToken cancellationToken)
    {
        if (await _perfilRepository.ExistsByCodigoAsync(request.Codigo, excludeId: null, cancellationToken))
        {
            throw new BusinessException($"Ya existe un perfil con el código '{request.Codigo}'.", "PERFIL_CODIGO_DUPLICADO");
        }

        var perfil = new Domain.Entities.Perfil
        {
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = true,
            UsuarioRegistro = request.UsuarioRegistro,
            FechaRegistro = DateTime.UtcNow
        };

        perfil.Id = await _perfilRepository.CreateAsync(perfil, cancellationToken);

        return perfil.ToDto();
    }
}
