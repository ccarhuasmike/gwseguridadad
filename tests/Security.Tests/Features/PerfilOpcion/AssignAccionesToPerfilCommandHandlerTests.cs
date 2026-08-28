using System.Data;
using Moq;
using Security.Application.Common.Interfaces;
using Security.Application.Features.PerfilOpcion.Commands.AssignAccionesToPerfil;
using Security.Domain.Entities;
using Security.Domain.Exceptions;

namespace Security.Tests.Features.PerfilOpcion;

public class AssignAccionesToPerfilCommandHandlerTests
{
    private readonly Mock<IPerfilRepository> _perfilRepository = new();
    private readonly Mock<IAccionRepository> _accionRepository = new();
    private readonly Mock<IPerfilOpcionRepository> _perfilOpcionRepository = new();
    private readonly Mock<IPerfilAccionRepository> _perfilAccionRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private AssignAccionesToPerfilCommandHandler CreateHandler() => new(
        _perfilRepository.Object,
        _accionRepository.Object,
        _perfilOpcionRepository.Object,
        _perfilAccionRepository.Object,
        _unitOfWork.Object);

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenPerfilDoesNotExist()
    {
        _perfilRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var command = new AssignAccionesToPerfilCommand(1, new List<int> { 10 }, 1);

        await Assert.ThrowsAsync<NotFoundException>(() => CreateHandler().Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenOpcionOfAccionIsNotAssignedToPerfil()
    {
        _perfilRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _accionRepository
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Accion { Id = 10, IdOpcion = 100, Nombre = "Crear" });
        _perfilOpcionRepository
            .Setup(r => r.ExistsAsync(1, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new AssignAccionesToPerfilCommand(1, new List<int> { 10 }, 1);

        var exception = await Assert.ThrowsAsync<BusinessException>(() => CreateHandler().Handle(command, CancellationToken.None));

        Assert.Equal("ACCION_OPCION_NO_ASIGNADA", exception.Code);
        _unitOfWork.Verify(
            u => u.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldAssignAccion_WhenItsOpcionIsAlreadyAssignedToPerfil()
    {
        _perfilRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _accionRepository
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Accion { Id = 10, IdOpcion = 100, Nombre = "Crear" });
        _perfilOpcionRepository
            .Setup(r => r.ExistsAsync(1, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _perfilAccionRepository
            .Setup(r => r.ExistsAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _unitOfWork
            .Setup(u => u.ExecuteAsync(It.IsAny<Func<IDbConnection, IDbTransaction, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<IDbConnection, IDbTransaction, Task>, CancellationToken>((operation, ct) => operation(Mock.Of<IDbConnection>(), Mock.Of<IDbTransaction>()));

        var command = new AssignAccionesToPerfilCommand(1, new List<int> { 10 }, 1);

        await CreateHandler().Handle(command, CancellationToken.None);

        _perfilAccionRepository.Verify(
            r => r.AddAsync(
                It.Is<PerfilAccion>(p => p.IdPerfil == 1 && p.IdAccion == 10),
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
