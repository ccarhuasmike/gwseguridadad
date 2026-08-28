using MediatR;
using Security.Application.Features.Opcion.DTOs;

namespace Security.Application.Features.Opcion.Commands.UpdateOpcion;

public record UpdateOpcionCommand(
    int Id,
    int? IdPadre,
    string Nombre,
    string Descripcion,
    string? Ruta,
    byte Orden,
    bool Visible,
    bool Activo,
    int UsuarioModifica) : IRequest<OpcionDto>;
