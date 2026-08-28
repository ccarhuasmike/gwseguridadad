namespace Security.Domain.Entities;

/// <summary>
/// Association between a Usuario and an Accion. Stored in table seg.UsuarioAccion.
/// </summary>
public class UsuarioAccion
{
    public int IdAccion { get; set; }

    public int IdUsuario { get; set; }

    public int IdCarga { get; set; }

    public int UsuarioRegistro { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
