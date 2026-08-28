using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Opcion.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.Opcion.Queries.GetOpcionById;

public class GetOpcionByIdQueryHandler : IRequestHandler<GetOpcionByIdQuery, OpcionDto>
{
    private readonly IOpcionRepository _opcionRepository;

    public GetOpcionByIdQueryHandler(IOpcionRepository opcionRepository)
    {
        _opcionRepository = opcionRepository;
    }

    public async Task<OpcionDto> Handle(GetOpcionByIdQuery request, CancellationToken cancellationToken)
    {
        var opcion = await _opcionRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw NotFoundException.For("Opcion", request.Id);

        return opcion.ToDto();
    }
}
