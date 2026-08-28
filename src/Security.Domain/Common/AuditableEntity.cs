namespace Security.Domain.Common;

/// <summary>
/// Base class providing the standard audit columns shared by every table in the
/// `seg` schema (UsuarioRegistro, FechaRegistro, UsuarioModifica, FechaModifica).
/// </summary>
public abstract class AuditableEntity
{
    public int UsuarioRegistro { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public int? UsuarioModifica { get; set; }

    public DateTime? FechaModifica { get; set; }
}
