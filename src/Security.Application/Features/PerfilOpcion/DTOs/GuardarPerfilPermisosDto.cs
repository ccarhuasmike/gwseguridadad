namespace Security.Application.Features.PerfilOpcion.DTOs;

/// <summary>Input payload sent from Angular to persist the full permission configuration of a Perfil in one operation.</summary>
public class GuardarPerfilPermisosDto
{
    public List<OpcionSeleccionadaDto> Opciones { get; set; } = new();
}

public class OpcionSeleccionadaDto
{
    public int IdOpcion { get; set; }

    public List<int> IdAcciones { get; set; } = new();
}
