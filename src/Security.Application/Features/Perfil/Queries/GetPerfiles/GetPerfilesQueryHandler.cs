using MediatR;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Perfil.DTOs;

namespace Security.Application.Features.Perfil.Queries.GetPerfiles;

public class GetPerfilesQueryHandler : IRequestHandler<GetPerfilesQuery, IReadOnlyList<PerfilDto>>
{
    private readonly IPerfilRepository _perfilRepository;

    public GetPerfilesQueryHandler(IPerfilRepository perfilRepository)
    {
        _perfilRepository = perfilRepository;
    }

    public async Task<IReadOnlyList<PerfilDto>> Handle(GetPerfilesQuery request, CancellationToken cancellationToken)
    {
        var perfiles = await _perfilRepository.GetAllAsync(request.IncludeInactive, cancellationToken);
        return perfiles.Select(p => p.ToDto()).ToList();
    }
}
