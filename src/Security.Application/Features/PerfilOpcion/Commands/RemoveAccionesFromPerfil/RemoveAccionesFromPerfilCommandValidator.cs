using FluentValidation;

namespace Security.Application.Features.PerfilOpcion.Commands.RemoveAccionesFromPerfil;

public class RemoveAccionesFromPerfilCommandValidator : AbstractValidator<RemoveAccionesFromPerfilCommand>
{
    public RemoveAccionesFromPerfilCommandValidator()
    {
        RuleFor(x => x.IdPerfil).GreaterThan(0);
        RuleFor(x => x.IdAcciones).NotEmpty();
    }
}
