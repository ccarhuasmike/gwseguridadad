namespace Security.Application.Features.Opcion.DTOs;

/// <summary>Represents an Opcion together with its recursive children, used to render the tree view.</summary>
public class OpcionTreeNodeDto
{
    public int Id { get; set; }

    public int? IdPadre { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public string? Ruta { get; set; }

    public byte Orden { get; set; }

    public bool Visible { get; set; }

    public bool Activo { get; set; }

    public List<OpcionTreeNodeDto> Hijos { get; set; } = new();
}
