using MediatR;
using Security.Application.Features.PerfilOpcion.DTOs;

namespace Security.Application.Features.PerfilOpcion.Commands.SavePerfilPermisos;

/// <summary>
/// Persists the complete permission configuration (Opciones + Acciones) of a
/// Perfil in a single transactional operation: existing PerfilOpcion/PerfilAccion
/// rows not present in the payload are removed, and missing ones are added.
/// </summary>
public record SavePerfilPermisosCommand(int IdPerfil, IReadOnlyList<OpcionSeleccionadaDto> Opciones, int UsuarioRegistro) : IRequest;
