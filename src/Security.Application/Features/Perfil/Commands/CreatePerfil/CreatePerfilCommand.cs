using MediatR;
using Security.Application.Features.Perfil.DTOs;

namespace Security.Application.Features.Perfil.Commands.CreatePerfil;

public record CreatePerfilCommand(string Codigo, string Nombre, string? Descripcion, int UsuarioRegistro) : IRequest<PerfilDto>;
