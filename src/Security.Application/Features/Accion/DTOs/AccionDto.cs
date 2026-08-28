namespace Security.Application.Features.Accion.DTOs;

public class AccionDto
{
    public int Id { get; set; }

    public int IdOpcion { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }
}

public static class AccionMappingExtensions
{
    public static AccionDto ToDto(this Domain.Entities.Accion accion) => new()
    {
        Id = accion.Id,
        IdOpcion = accion.IdOpcion,
        Nombre = accion.Nombre,
        Descripcion = accion.Descripcion,
        Activo = accion.Activo
    };
}
