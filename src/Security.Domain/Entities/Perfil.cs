using Security.Domain.Common;

namespace Security.Domain.Entities;

/// <summary>
/// Represents a security profile (role) stored in table seg.Perfil.
/// Assumption: mirrors the provided dbo.Perfil conceptual model.
/// </summary>
public class Perfil : AuditableEntity
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;
}
