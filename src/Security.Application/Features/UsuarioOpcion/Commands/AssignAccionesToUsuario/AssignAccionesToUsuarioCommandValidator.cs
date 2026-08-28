using FluentValidation;

namespace Security.Application.Features.UsuarioOpcion.Commands.AssignAccionesToUsuario;

public class AssignAccionesToUsuarioCommandValidator : AbstractValidator<AssignAccionesToUsuarioCommand>
{
    public AssignAccionesToUsuarioCommandValidator()
    {
        RuleFor(x => x.IdUsuario).GreaterThan(0);
        RuleFor(x => x.IdAcciones).NotEmpty();
    }
}
