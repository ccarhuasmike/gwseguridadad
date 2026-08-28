using Security.Domain.Common;

namespace Security.Domain.Entities;

/// <summary>
/// Represents an action (e.g. Crear/Editar/Consultar/Eliminar) that belongs to
/// exactly one Opcion. Stored in table seg.Accion.
/// </summary>
public class Accion : AuditableEntity
{
    public int Id { get; set; }

    public int IdOpcion { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;
}
