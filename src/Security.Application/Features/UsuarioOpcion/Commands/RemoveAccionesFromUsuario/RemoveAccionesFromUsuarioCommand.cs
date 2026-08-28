using MediatR;

namespace Security.Application.Features.UsuarioOpcion.Commands.RemoveAccionesFromUsuario;

public record RemoveAccionesFromUsuarioCommand(int IdUsuario, IReadOnlyList<int> IdAcciones) : IRequest;
