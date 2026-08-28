using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Opcion.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.Opcion.Commands.CreateOpcion;

public class CreateOpcionCommandHandler : IRequestHandler<CreateOpcionCommand, OpcionDto>
{
    private readonly IOpcionRepository _opcionRepository;

    public CreateOpcionCommandHandler(IOpcionRepository opcionRepository)
    {
        _opcionRepository = opcionRepository;
    }

    public async Task<OpcionDto> Handle(CreateOpcionCommand request, CancellationToken cancellationToken)
    {
        if (request.IdPadre.HasValue && !await _opcionRepository.ExistsAsync(request.IdPadre.Value, cancellationToken))
        {
            throw NotFoundException.For("Opcion padre", request.IdPadre.Value);
        }

        var opcion = new Domain.Entities.Opcion
        {
            IdPadre = request.IdPadre,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Ruta = request.Ruta,
            Orden = request.Orden,
            Visible = request.Visible,
            Activo = true,
            UsuarioRegistro = request.UsuarioRegistro,
            FechaRegistro = DateTime.UtcNow
        };

        opcion.Id = await _opcionRepository.CreateAsync(opcion, cancellationToken);

        return opcion.ToDto();
    }
}
