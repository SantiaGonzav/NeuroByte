using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EquipService.Domain.Entities
{
    // ✅ Enum limpio, sin espacios en los nombres internos
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EquipoEstado
    {
        Disponible,
        Ocupado,
        Mantenimiento
    }

    public class EquipoMedico
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(120)]
        public string Modelo { get; set; } = string.Empty;

        [Required, MaxLength(120)]
        public string NumeroSerie { get; set; } = string.Empty;

        [Required]
        public DateTime FechaAdquisicion { get; set; }

        [Required]
        public EquipoEstado Estado { get; set; } = EquipoEstado.Disponible;

        [MaxLength(500)]
        public string? Imagen { get; set; }
    }
}


