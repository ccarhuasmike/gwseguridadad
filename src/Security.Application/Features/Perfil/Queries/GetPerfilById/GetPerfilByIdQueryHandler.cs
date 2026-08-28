using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Perfil.DTOs;
using Security.Domain.Exceptions;

namespace Security.Application.Features.Perfil.Queries.GetPerfilById;

public class GetPerfilByIdQueryHandler : IRequestHandler<GetPerfilByIdQuery, PerfilDto>
{
    private readonly IPerfilRepository _perfilRepository;

    public GetPerfilByIdQueryHandler(IPerfilRepository perfilRepository)
    {
        _perfilRepository = perfilRepository;
    }

    public async Task<PerfilDto> Handle(GetPerfilByIdQuery request, CancellationToken cancellationToken)
    {
        var perfil = await _perfilRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw NotFoundException.For("Perfil", request.Id);

        return perfil.ToDto();
    }
}
