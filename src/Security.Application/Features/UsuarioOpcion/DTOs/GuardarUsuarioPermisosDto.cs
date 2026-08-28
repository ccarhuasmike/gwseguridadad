namespace Security.Application.Features.UsuarioOpcion.DTOs;

/// <summary>Input payload sent from Angular to persist the full permission configuration of a Usuario in one operation.</summary>
public class GuardarUsuarioPermisosDto
{
    public List<OpcionSeleccionadaUsuarioDto> Opciones { get; set; } = new();
}

public class OpcionSeleccionadaUsuarioDto
{
    public int IdOpcion { get; set; }

    public List<int> IdAcciones { get; set; } = new();
}
