using MediatR;

namespace Security.Application.Features.Opcion.Commands.DeleteOpcion;

public record DeleteOpcionCommand(int Id, int UsuarioModifica) : IRequest;
