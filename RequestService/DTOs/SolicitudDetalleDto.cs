using RequestService.HttpClients;

public class SolicitudDetalleDto
{
    public int Id { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime FechaProgramada { get; set; }
    public string Procedimiento { get; set; }
    public string Observaciones { get; set; }
    public string Estado { get; set; }

    public int EquipoId { get; set; }
    public EquipoDto Equipo { get; set; }

    public string UserId { get; set; }
    public string? ReviewedById { get; set; }

    // Ya estaban
    public UsuarioDto Usuario { get; set; }
    public UsuarioDto RevisadoPorUsuario { get; set; }
}
