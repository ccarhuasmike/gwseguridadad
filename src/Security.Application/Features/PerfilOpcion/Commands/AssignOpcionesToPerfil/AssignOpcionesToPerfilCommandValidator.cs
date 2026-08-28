using FluentValidation;

namespace Security.Application.Features.PerfilOpcion.Commands.AssignOpcionesToPerfil;

public class AssignOpcionesToPerfilCommandValidator : AbstractValidator<AssignOpcionesToPerfilCommand>
{
    public AssignOpcionesToPerfilCommandValidator()
    {
        RuleFor(x => x.IdPerfil).GreaterThan(0);
        RuleFor(x => x.IdOpciones).NotEmpty();
    }
}
