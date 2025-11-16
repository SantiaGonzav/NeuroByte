using System.ComponentModel.DataAnnotations;

namespace EquipService.Application.DTOs
{
    public class EquipoMedicoDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Modelo { get; set; } = string.Empty;

        [Required]
        public string NumeroSerie { get; set; } = string.Empty;

        [Required]
        public DateTime FechaAdquisicion { get; set; }

        [Required]
        public string Estado { get; set; } = "Disponible";

        public string Imagen { get; set; } = string.Empty;
    }
}

