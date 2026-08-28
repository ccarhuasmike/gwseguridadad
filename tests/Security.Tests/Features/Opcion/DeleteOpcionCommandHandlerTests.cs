using Moq;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Opcion.Commands.DeleteOpcion;
using Security.Domain.Exceptions;

namespace Security.Tests.Features.Opcion;

public class DeleteOpcionCommandHandlerTests
{
    private readonly Mock<IOpcionRepository> _opcionRepository = new();

    private DeleteOpcionCommandHandler CreateHandler() => new(_opcionRepository.Object);

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenOpcionDoesNotExist()
    {
        _opcionRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<NotFoundException>(() => CreateHandler().Handle(new DeleteOpcionCommand(1, 1), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenOpcionHasActiveChildren()
    {
        _opcionRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _opcionRepository.Setup(r => r.HasChildrenAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => CreateHandler().Handle(new DeleteOpcionCommand(1, 1), CancellationToken.None));

        Assert.Equal("OPCION_TIENE_HIJOS", exception.Code);
        _opcionRepository.Verify(r => r.DeactivateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenOpcionHasActiveAcciones()
    {
        _opcionRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _opcionRepository.Setup(r => r.HasChildrenAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _opcionRepository.Setup(r => r.HasAccionesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => CreateHandler().Handle(new DeleteOpcionCommand(1, 1), CancellationToken.None));

        Assert.Equal("OPCION_TIENE_ACCIONES", exception.Code);
        _opcionRepository.Verify(r => r.DeactivateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDeactivate_WhenOpcionHasNoDependencies()
    {
        _opcionRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _opcionRepository.Setup(r => r.HasChildrenAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _opcionRepository.Setup(r => r.HasAccionesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await CreateHandler().Handle(new DeleteOpcionCommand(1, 7), CancellationToken.None);

        _opcionRepository.Verify(r => r.DeactivateAsync(1, 7, It.IsAny<CancellationToken>()), Times.Once);
    }
}
