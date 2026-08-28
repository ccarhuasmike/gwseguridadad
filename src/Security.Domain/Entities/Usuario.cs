using Security.Domain.Common;

namespace Security.Domain.Entities;

/// <summary>
/// Minimal representation of a system user stored in table seg.Usuario.
/// Assumption: the real Usuario table (authentication, credentials, etc.) is
/// owned by another module; only the columns required to validate the
/// existence of a user for permission assignment purposes are modeled here.
/// </summary>
public class Usuario : AuditableEntity
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Login { get; set; }

    public bool Activo { get; set; } = true;
}
