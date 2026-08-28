using Security.Domain.Common;

namespace Security.Domain.Entities;

/// <summary>
/// Represents a recursive menu/permission option stored in table seg.Opcion.
/// An option without <see cref="IdPadre"/> is a root option; otherwise it is a
/// sub-option of another Opcion.
/// </summary>
public class Opcion : AuditableEntity
{
    public int Id { get; set; }

    public int? IdPadre { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public string? Ruta { get; set; }

    public byte Orden { get; set; }

    public bool Visible { get; set; } = true;

    public bool Activo { get; set; } = true;
}
