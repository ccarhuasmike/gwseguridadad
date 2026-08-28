using MediatR;

namespace Security.Application.Features.PerfilOpcion.Commands.RemoveAccionesFromPerfil;

public record RemoveAccionesFromPerfilCommand(int IdPerfil, IReadOnlyList<int> IdAcciones) : IRequest;
