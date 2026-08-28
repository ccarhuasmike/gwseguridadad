using FluentValidation;

namespace Security.Application.Features.Accion.Commands.DeleteAccion;

public class DeleteAccionCommandValidator : AbstractValidator<DeleteAccionCommand>
{
    public DeleteAccionCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
