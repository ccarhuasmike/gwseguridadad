using System.Data;
using Moq;
using Security.Application.Common.Interfaces;
using Security.Application.Features.PerfilOpcion.Commands.SavePerfilPermisos;
using Security.Application.Features.PerfilOpcion.DTOs;
using Security.Domain.Exceptions;

namespace Security.Tests.Features.PerfilOpcion;

public class SavePerfilPermisosCommandHandlerTests
{
    private readonly Mock<IPerfilRepository> _perfilRepository = new();
    private readonly Mock<IOpcionRepository> _opcionRepository = new();
    private readonly Mock<IAccionRepository> _accionRepository = new();
    private readonly Mock<IPerfilOpcionRepository> _perfilOpcionRepository = new();
    private readonly Mock<IPerfilAccionRepository> _perfilAccionRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private SavePerfilPermisosCommandHandler CreateHandler() => new(
        _perfilRepository.Object,
        _opcionRepository.Object,
        _accionRepository.Object,
        _perfilOpcionRepository.Object,
        _perfilAccionRepository.Object,
        _unitOfWork.Object);

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenPerfilDoesNotExist()
    {
        _perfilRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var command = new SavePerfilPermisosCommand(1, new List<OpcionSeleccionadaDto>(), 1);

        await Assert.ThrowsAsync<NotFoundException>(() => CreateHandler().Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenAccionDoesNotBelongToOpcion()
    {
        _perfilRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _opcionRepository.Setup(r => r.ExistsAsync(100, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _accionRepository.Setup(r => r.BelongsToOpcionAsync(10, 100, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var command = new SavePerfilPermisosCommand(
            1,
            new List<OpcionSeleccionadaDto> { new() { IdOpcion = 100, IdAcciones = new List<int> { 10 } } },
            1);

        var exception = await Assert.ThrowsAsync<BusinessException>(() => CreateHandler().Handle(command, CancellationToken.None));

        Assert.Equal("ACCION_NO_PERTENECE_A_OPCION", exception.Code);
        _unitOfWork.Verify(
            u => u.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSaveConfiguration_WhenAllAccionesBelongToTheirOpciones()
    {
        _perfilRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _opcionRepository.Setup(r => r.ExistsAsync(100, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _accionRepository.Setup(r => r.BelongsToOpcionAsync(10, 100, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _unitOfWork
            .Setup(u => u.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<IDbConnection, IDbTransaction, Task>, CancellationToken>((operation, ct) => operation(Mock.Of<IDbConnection>(), Mock.Of<IDbTransaction>()));

        var command = new SavePerfilPermisosCommand(
            1,
            new List<OpcionSeleccionadaDto> { new() { IdOpcion = 100, IdAcciones = new List<int> { 10 } } },
            1);

        await CreateHandler().Handle(command, CancellationToken.None);

        _perfilOpcionRepository.Verify(r => r.RemoveAllForPerfilAsync(1, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>(), It.IsAny<CancellationToken>()), Times.Once);
        _perfilAccionRepository.Verify(r => r.RemoveAllForPerfilAsync(1, It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>(), It.IsAny<CancellationToken>()), Times.Once);
        _perfilOpcionRepository.Verify(
            r => r.AddAsync(It.Is<Domain.Entities.PerfilOpcion>(p => p.IdPerfil == 1 && p.IdOpcion == 100), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _perfilAccionRepository.Verify(
            r => r.AddAsync(It.Is<Domain.Entities.PerfilAccion>(p => p.IdPerfil == 1 && p.IdAccion == 10), It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
