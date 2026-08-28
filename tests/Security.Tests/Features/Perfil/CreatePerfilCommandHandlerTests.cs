using Moq;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Perfil.Commands.CreatePerfil;
using Security.Domain.Exceptions;

namespace Security.Tests.Features.Perfil;

public class CreatePerfilCommandHandlerTests
{
    private readonly Mock<IPerfilRepository> _perfilRepository = new();

    private CreatePerfilCommandHandler CreateHandler() => new(_perfilRepository.Object);

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenCodigoAlreadyExists()
    {
        _perfilRepository
            .Setup(r => r.ExistsByCodigoAsync("ADM", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreatePerfilCommand("ADM", "Administrador", null, UsuarioRegistro: 1);

        var exception = await Assert.ThrowsAsync<BusinessException>(() => CreateHandler().Handle(command, CancellationToken.None));

        Assert.Equal("PERFIL_CODIGO_DUPLICADO", exception.Code);
        _perfilRepository.Verify(r => r.CreateAsync(It.IsAny<Domain.Entities.Perfil>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreatePerfil_WhenCodigoIsUnique()
    {
        _perfilRepository
            .Setup(r => r.ExistsByCodigoAsync("ADM", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _perfilRepository
            .Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.Perfil>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

        var command = new CreatePerfilCommand("ADM", "Administrador", "Perfil administrador", UsuarioRegistro: 1);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.Equal(42, result.Id);
        Assert.Equal("ADM", result.Codigo);
        Assert.Equal("Administrador", result.Nombre);
        Assert.True(result.Activo);
    }
}
