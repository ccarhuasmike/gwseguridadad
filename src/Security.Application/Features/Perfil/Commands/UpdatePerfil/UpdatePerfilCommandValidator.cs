using FluentValidation;

namespace Security.Application.Features.Perfil.Commands.UpdatePerfil;

public class UpdatePerfilCommandValidator : AbstractValidator<UpdatePerfilCommand>
{
    public UpdatePerfilCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).MaximumLength(250);
    }
}
