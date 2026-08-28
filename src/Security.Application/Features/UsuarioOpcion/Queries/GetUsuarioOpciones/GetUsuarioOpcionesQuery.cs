using MediatR;
using Security.Application.Features.Opcion.DTOs;

namespace Security.Application.Features.UsuarioOpcion.Queries.GetUsuarioOpciones;

public record GetUsuarioOpcionesQuery(int IdUsuario) : IRequest<IReadOnlyList<OpcionDto>>;
