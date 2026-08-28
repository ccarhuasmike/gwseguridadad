using FluentValidation;

namespace Security.Application.Features.Perfil.Commands.DeletePerfil;

public class DeletePerfilCommandValidator : AbstractValidator<DeletePerfilCommand>
{
    public DeletePerfilCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
