using FluentValidation;

namespace Security.Application.Features.UsuarioOpcion.Commands.RemoveAccionesFromUsuario;

public class RemoveAccionesFromUsuarioCommandValidator : AbstractValidator<RemoveAccionesFromUsuarioCommand>
{
    public RemoveAccionesFromUsuarioCommandValidator()
    {
        RuleFor(x => x.IdUsuario).GreaterThan(0);
        RuleFor(x => x.IdAcciones).NotEmpty();
    }
}
