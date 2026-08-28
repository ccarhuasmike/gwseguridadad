using FluentValidation;

namespace Security.Application.Features.Perfil.Commands.CreatePerfil;

public class CreatePerfilCommandValidator : AbstractValidator<CreatePerfilCommand>
{
    public CreatePerfilCommandValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).MaximumLength(250);
    }
}
