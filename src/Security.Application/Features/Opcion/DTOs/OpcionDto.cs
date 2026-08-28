namespace Security.Application.Features.Opcion.DTOs;

public class OpcionDto
{
    public int Id { get; set; }

    public int? IdPadre { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public string? Ruta { get; set; }

    public byte Orden { get; set; }

    public bool Visible { get; set; }

    public bool Activo { get; set; }
}

public static class OpcionMappingExtensions
{
    public static OpcionDto ToDto(this Domain.Entities.Opcion opcion) => new()
    {
        Id = opcion.Id,
        IdPadre = opcion.IdPadre,
        Nombre = opcion.Nombre,
        Descripcion = opcion.Descripcion,
        Ruta = opcion.Ruta,
        Orden = opcion.Orden,
        Visible = opcion.Visible,
        Activo = opcion.Activo
    };
}
