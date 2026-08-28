using FluentValidation;

namespace Security.Application.Features.UsuarioOpcion.Commands.SaveUsuarioPermisos;

public class SaveUsuarioPermisosCommandValidator : AbstractValidator<SaveUsuarioPermisosCommand>
{
    public SaveUsuarioPermisosCommandValidator()
    {
        RuleFor(x => x.IdUsuario).GreaterThan(0);
        RuleForEach(x => x.Opciones).ChildRules(opcion =>
        {
            opcion.RuleFor(o => o.IdOpcion).GreaterThan(0);
        });
    }
}
