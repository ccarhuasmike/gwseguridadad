using FluentValidation;

namespace Security.Application.Features.Accion.Commands.UpdateAccion;

public class UpdateAccionCommandValidator : AbstractValidator<UpdateAccionCommand>
{
    public UpdateAccionCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).MaximumLength(250);
    }
}
