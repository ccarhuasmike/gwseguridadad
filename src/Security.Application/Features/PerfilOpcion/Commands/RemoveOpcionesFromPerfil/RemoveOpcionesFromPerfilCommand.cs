using MediatR;

namespace Security.Application.Features.PerfilOpcion.Commands.RemoveOpcionesFromPerfil;

public record RemoveOpcionesFromPerfilCommand(int IdPerfil, IReadOnlyList<int> IdOpciones) : IRequest;
