using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Accion.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.Accion.Commands.CreateAccion;

public class CreateAccionCommandHandler : IRequestHandler<CreateAccionCommand, AccionDto>
{
    private readonly IAccionRepository _accionRepository;
    private readonly IOpcionRepository _opcionRepository;

    public CreateAccionCommandHandler(IAccionRepository accionRepository, IOpcionRepository opcionRepository)
    {
        _accionRepository = accionRepository;
        _opcionRepository = opcionRepository;
    }

    public async Task<AccionDto> Handle(CreateAccionCommand request, CancellationToken cancellationToken)
    {
        if (!await _opcionRepository.ExistsAsync(request.IdOpcion, cancellationToken))
        {
            throw NotFoundException.For("Opcion", request.IdOpcion);
        }

        var accion = new Domain.Entities.Accion
        {
            IdOpcion = request.IdOpcion,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = true,
            UsuarioRegistro = request.UsuarioRegistro,
            FechaRegistro = DateTime.UtcNow
        };

        accion.Id = await _accionRepository.CreateAsync(accion, cancellationToken);

        return accion.ToDto();
    }
}
