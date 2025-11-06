namespace EquipService.Application.DTOs
{
    public class EquipoMedicoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string NumeroSerie { get; set; } = string.Empty;
        public DateTime FechaAdquisicion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
    }
}

