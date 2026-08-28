namespace Security.Domain.Entities;

/// <summary>
/// Association between a Usuario and an Opcion. Stored in table seg.UsuarioOpcion.
/// </summary>
public class UsuarioOpcion
{
    public int IdUsuario { get; set; }

    public int IdOpcion { get; set; }

    public int IdCarga { get; set; }

    public int UsuarioRegistro { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
