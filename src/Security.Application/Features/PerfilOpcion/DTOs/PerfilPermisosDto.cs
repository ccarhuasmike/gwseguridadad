namespace Security.Application.Features.PerfilOpcion.DTOs;

/// <summary>Full permission tree (Opciones > SubOpciones > Acciones) configured for a Perfil.</summary>
public class PerfilPermisosDto
{
    public int IdPerfil { get; set; }

    public List<OpcionPermisoDto> Opciones { get; set; } = new();
}

public class OpcionPermisoDto
{
    public int Id { get; set; }

    public int? IdPadre { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public bool Seleccionado { get; set; }

    public List<AccionPermisoDto> Acciones { get; set; } = new();

    public List<OpcionPermisoDto> Hijos { get; set; } = new();
}

public class AccionPermisoDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public bool Seleccionado { get; set; }
}
