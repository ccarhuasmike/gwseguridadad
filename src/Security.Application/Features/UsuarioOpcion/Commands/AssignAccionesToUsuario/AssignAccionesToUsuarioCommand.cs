using MediatR;

namespace Security.Application.Features.UsuarioOpcion.Commands.AssignAccionesToUsuario;

public record AssignAccionesToUsuarioCommand(int IdUsuario, IReadOnlyList<int> IdAcciones, int UsuarioRegistro) : IRequest;
