namespace Security.Application.Features.Perfil.DTOs;

public class PerfilDto
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }
}

public static class PerfilMappingExtensions
{
    public static PerfilDto ToDto(this Domain.Entities.Perfil perfil) => new()
    {
        Id = perfil.Id,
        Codigo = perfil.Codigo,
        Nombre = perfil.Nombre,
        Descripcion = perfil.Descripcion,
        Activo = perfil.Activo
    };
}
