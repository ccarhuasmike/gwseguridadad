using FluentValidation;

namespace Security.Application.Features.PerfilOpcion.Commands.AssignAccionesToPerfil;

public class AssignAccionesToPerfilCommandValidator : AbstractValidator<AssignAccionesToPerfilCommand>
{
    public AssignAccionesToPerfilCommandValidator()
    {
        RuleFor(x => x.IdPerfil).GreaterThan(0);
        RuleFor(x => x.IdAcciones).NotEmpty();
    }
}
