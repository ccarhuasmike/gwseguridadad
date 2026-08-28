using MediatR;

namespace Security.Application.Features.UsuarioOpcion.Commands.RemoveOpcionesFromUsuario;

public record RemoveOpcionesFromUsuarioCommand(int IdUsuario, IReadOnlyList<int> IdOpciones) : IRequest;
