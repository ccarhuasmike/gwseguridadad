using MediatR;

namespace Security.Application.Features.UsuarioOpcion.Commands.AssignOpcionesToUsuario;

public record AssignOpcionesToUsuarioCommand(int IdUsuario, IReadOnlyList<int> IdOpciones, int UsuarioRegistro) : IRequest;
