using FluentValidation;

namespace Security.Application.Features.PerfilOpcion.Commands.RemoveOpcionesFromPerfil;

public class RemoveOpcionesFromPerfilCommandValidator : AbstractValidator<RemoveOpcionesFromPerfilCommand>
{
    public RemoveOpcionesFromPerfilCommandValidator()
    {
        RuleFor(x => x.IdPerfil).GreaterThan(0);
        RuleFor(x => x.IdOpciones).NotEmpty();
    }
}
