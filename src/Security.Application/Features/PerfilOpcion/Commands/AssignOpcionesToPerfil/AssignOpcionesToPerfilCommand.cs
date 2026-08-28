using MediatR;

namespace Security.Application.Features.PerfilOpcion.Commands.AssignOpcionesToPerfil;

public record AssignOpcionesToPerfilCommand(int IdPerfil, IReadOnlyList<int> IdOpciones, int UsuarioRegistro) : IRequest;
