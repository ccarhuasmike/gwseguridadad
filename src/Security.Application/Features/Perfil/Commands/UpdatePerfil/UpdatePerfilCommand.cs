using MediatR;
using Security.Application.Features.Perfil.DTOs;

namespace Security.Application.Features.Perfil.Commands.UpdatePerfil;

public record UpdatePerfilCommand(int Id, string Codigo, string Nombre, string? Descripcion, bool Activo, int UsuarioModifica) : IRequest<PerfilDto>;
