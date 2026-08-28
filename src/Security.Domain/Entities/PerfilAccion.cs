namespace Security.Domain.Entities;

/// <summary>
/// Association between a Perfil and an Accion. Stored in table seg.PerfilAccion.
/// </summary>
public class PerfilAccion
{
    public int IdPerfil { get; set; }

    public int IdAccion { get; set; }

    public int IdCarga { get; set; }

    public int UsuarioRegistro { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
