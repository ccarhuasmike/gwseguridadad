using MediatR;

namespace Security.Application.Features.Accion.Commands.DeleteAccion;

public record DeleteAccionCommand(int Id, int UsuarioModifica) : IRequest;
