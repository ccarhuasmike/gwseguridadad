using Moq;
using Security.Application.Common.Interfaces;
using Security.Application.Features.Opcion.Commands.UpdateOpcion;
using Security.Domain.Entities;
using Security.Domain.Exceptions;

namespace Security.Tests.Features.Opcion;

public class UpdateOpcionCommandHandlerTests
{
    private readonly Mock<IOpcionRepository> _opcionRepository = new();

    private UpdateOpcionCommandHandler CreateHandler() => new(_opcionRepository.Object);

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenNewParentIsADescendant_Cycle()
    {
        // Opcion 1 is being edited; Opcion 5 is one of its descendants.
        // Reassigning Opcion 1's parent to Opcion 5 would create a cycle.
        _opcionRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Entities.Opcion { Id = 1, Nombre = "Usuarios", Descripcion = "Usuarios" });
        _opcionRepository
            .Setup(r => r.ExistsAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _opcionRepository
            .Setup(r => r.GetAncestorIdsAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<int> { 1, 10 });

        var command = new UpdateOpcionCommand(
            Id: 1,
            IdPadre: 5,
            Nombre: "Usuarios",
            Descripcion: "Usuarios",
            Ruta: null,
            Orden: 0,
            Visible: true,
            Activo: true,
            UsuarioModifica: 1);

        var exception = await Assert.ThrowsAsync<BusinessException>(() => CreateHandler().Handle(command, CancellationToken.None));

        Assert.Equal("OPCION_CICLO_INVALIDO", exception.Code);
        _opcionRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Opcion>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenOpcionDoesNotExist()
    {
        _opcionRepository
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Opcion?)null);

        var command = new UpdateOpcionCommand(99, null, "X", "X", null, 0, true, true, 1);

        await Assert.ThrowsAsync<NotFoundException>(() => CreateHandler().Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldUpdate_WhenNewParentDoesNotCreateACycle()
    {
        _opcionRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Entities.Opcion { Id = 1, Nombre = "Usuarios", Descripcion = "Usuarios" });
        _opcionRepository
            .Setup(r => r.ExistsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _opcionRepository
            .Setup(r => r.GetAncestorIdsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<int>());

        var command = new UpdateOpcionCommand(1, 2, "Usuarios", "Usuarios", null, 0, true, true, 1);

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.Equal(2, result.IdPadre);
        _opcionRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Opcion>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
