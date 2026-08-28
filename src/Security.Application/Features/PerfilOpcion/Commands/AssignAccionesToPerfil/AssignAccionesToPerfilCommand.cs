using MediatR;

namespace Security.Application.Features.PerfilOpcion.Commands.AssignAccionesToPerfil;

public record AssignAccionesToPerfilCommand(int IdPerfil, IReadOnlyList<int> IdAcciones, int UsuarioRegistro) : IRequest;
