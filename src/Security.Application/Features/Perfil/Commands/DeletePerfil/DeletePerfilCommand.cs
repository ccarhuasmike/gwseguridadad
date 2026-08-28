using MediatR;

namespace Security.Application.Features.Perfil.Commands.DeletePerfil;

public record DeletePerfilCommand(int Id, int UsuarioModifica) : IRequest;
