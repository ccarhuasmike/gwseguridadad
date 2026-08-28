using FluentValidation;

namespace Security.Application.Features.Opcion.Commands.DeleteOpcion;

public class DeleteOpcionCommandValidator : AbstractValidator<DeleteOpcionCommand>
{
    public DeleteOpcionCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
