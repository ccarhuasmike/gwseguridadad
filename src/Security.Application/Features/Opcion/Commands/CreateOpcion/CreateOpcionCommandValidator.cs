using FluentValidation;

namespace Security.Application.Features.Opcion.Commands.CreateOpcion;

public class CreateOpcionCommandValidator : AbstractValidator<CreateOpcionCommand>
{
    public CreateOpcionCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Ruta).MaximumLength(250);
        RuleFor(x => x.IdPadre).GreaterThan(0).When(x => x.IdPadre.HasValue);
    }
}
