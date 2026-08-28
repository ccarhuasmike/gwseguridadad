using FluentValidation;

namespace Security.Application.Features.UsuarioOpcion.Commands.RemoveOpcionesFromUsuario;

public class RemoveOpcionesFromUsuarioCommandValidator : AbstractValidator<RemoveOpcionesFromUsuarioCommand>
{
    public RemoveOpcionesFromUsuarioCommandValidator()
    {
        RuleFor(x => x.IdUsuario).GreaterThan(0);
        RuleFor(x => x.IdOpciones).NotEmpty();
    }
}
