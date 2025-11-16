namespace RequestService.DTOs
{
    public class CrearSolicitudDto
    {
        public int EquipoId { get; set; }
        public DateTime FechaProgramada { get; set; }
        public string Procedimiento { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }
}
