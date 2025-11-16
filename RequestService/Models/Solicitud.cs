namespace RequestService.Models
{
    public class Solicitud
    {
        public int Id { get; set; }

        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

        public DateTime FechaProgramada { get; set; }

        public string Procedimiento { get; set; } = string.Empty;

        public string Estado { get; set; } = "Pendiente"; // Pendiente | Aprobada | Rechazada

        public string? Observaciones { get; set; }

        // Equipo
        public int EquipoId { get; set; }

        // Revisión
        public DateTime? FechaRevision { get; set; }

        // 🔥 IDs numéricos, como antes
        public int UserId { get; set; }        // usuario creador
        public int? ReviewedById { get; set; } // admin revisor
    }
}
