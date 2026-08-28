using FluentValidation;

namespace Security.Application.Features.Opcion.Commands.UpdateOpcion;

public class UpdateOpcionCommandValidator : AbstractValidator<UpdateOpcionCommand>
{
    public UpdateOpcionCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Ruta).MaximumLength(250);
        RuleFor(x => x.IdPadre).GreaterThan(0).When(x => x.IdPadre.HasValue);
        RuleFor(x => x).Must(x => x.IdPadre != x.Id)
            .WithMessage("Una opción no puede ser padre de sí misma.")
            .WithName(nameof(UpdateOpcionCommand.IdPadre));
    }
}
