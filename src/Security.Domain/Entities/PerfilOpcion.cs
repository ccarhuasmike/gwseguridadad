namespace Security.Domain.Entities;

/// <summary>
/// Association between a Perfil and an Opcion. Stored in table seg.PerfilOpcion.
/// </summary>
public class PerfilOpcion
{
    public int IdPerfil { get; set; }

    public int IdOpcion { get; set; }

    public int IdCarga { get; set; }

    public int UsuarioRegistro { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
