namespace Security.Application.Features.UsuarioOpcion.DTOs;

/// <summary>Full permission tree (Opciones > SubOpciones > Acciones) configured for a Usuario.</summary>
public class UsuarioPermisosDto
{
    public int IdUsuario { get; set; }

    public List<OpcionPermisoUsuarioDto> Opciones { get; set; } = new();
}

public class OpcionPermisoUsuarioDto
{
    public int Id { get; set; }

    public int? IdPadre { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public bool Seleccionado { get; set; }

    public List<AccionPermisoUsuarioDto> Acciones { get; set; } = new();

    public List<OpcionPermisoUsuarioDto> Hijos { get; set; } = new();
}

public class AccionPermisoUsuarioDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public bool Seleccionado { get; set; }
}
