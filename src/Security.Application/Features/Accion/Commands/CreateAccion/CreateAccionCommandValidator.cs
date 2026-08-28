using FluentValidation;

namespace Security.Application.Features.Accion.Commands.CreateAccion;

public class CreateAccionCommandValidator : AbstractValidator<CreateAccionCommand>
{
    public CreateAccionCommandValidator()
    {
        RuleFor(x => x.IdOpcion).GreaterThan(0).WithMessage("Toda acción debe pertenecer a una opción válida.");
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).MaximumLength(250);
    }
}
