using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Accion.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.Accion.Commands.UpdateAccion;

public class UpdateAccionCommandHandler : IRequestHandler<UpdateAccionCommand, AccionDto>
{
    private readonly IAccionRepository _accionRepository;

    public UpdateAccionCommandHandler(IAccionRepository accionRepository)
    {
        _accionRepository = accionRepository;
    }

    public async Task<AccionDto> Handle(UpdateAccionCommand request, CancellationToken cancellationToken)
    {
        var accion = await _accionRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw NotFoundException.For("Accion", request.Id);

        accion.Nombre = request.Nombre;
        accion.Descripcion = request.Descripcion;
        accion.Activo = request.Activo;
        accion.UsuarioModifica = request.UsuarioModifica;
        accion.FechaModifica = DateTime.UtcNow;

        await _accionRepository.UpdateAsync(accion, cancellationToken);

        return accion.ToDto();
    }
}
