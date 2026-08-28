using MediatR;
using Security.Application.Features.UsuarioOpcion.DTOs;

namespace Security.Application.Features.UsuarioOpcion.Commands.SaveUsuarioPermisos;

/// <summary>
/// Persists the complete permission configuration (Opciones + Acciones) of a
/// Usuario in a single transactional operation.
/// </summary>
public record SaveUsuarioPermisosCommand(int IdUsuario, IReadOnlyList<OpcionSeleccionadaUsuarioDto> Opciones, int UsuarioRegistro) : IRequest;
