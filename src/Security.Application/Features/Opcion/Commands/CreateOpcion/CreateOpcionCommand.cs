using MediatR;
using Security.Application.Features.Opcion.DTOs;

namespace Security.Application.Features.Opcion.Commands.CreateOpcion;

/// <summary>Creates a root Opcion when IdPadre is null, or a sub-opcion otherwise.</summary>
public record CreateOpcionCommand(
    int? IdPadre,
    string Nombre,
    string Descripcion,
    string? Ruta,
    byte Orden,
    bool Visible,
    int UsuarioRegistro) : IRequest<OpcionDto>;
