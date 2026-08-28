using FluentValidation;

namespace Security.Application.Features.PerfilOpcion.Commands.SavePerfilPermisos;

public class SavePerfilPermisosCommandValidator : AbstractValidator<SavePerfilPermisosCommand>
{
    public SavePerfilPermisosCommandValidator()
    {
        RuleFor(x => x.IdPerfil).GreaterThan(0);
        RuleForEach(x => x.Opciones).ChildRules(opcion =>
        {
            opcion.RuleFor(o => o.IdOpcion).GreaterThan(0);
        });
    }
}
