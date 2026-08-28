using FluentValidation;

namespace Security.Application.Features.UsuarioOpcion.Commands.AssignOpcionesToUsuario;

public class AssignOpcionesToUsuarioCommandValidator : AbstractValidator<AssignOpcionesToUsuarioCommand>
{
    public AssignOpcionesToUsuarioCommandValidator()
    {
        RuleFor(x => x.IdUsuario).GreaterThan(0);
        RuleFor(x => x.IdOpciones).NotEmpty();
    }
}
