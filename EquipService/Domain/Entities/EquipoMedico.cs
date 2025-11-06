namespace EquipService.Domain.Entities
{
    public class EquipoMedico
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string NumeroSerie { get; set; } = string.Empty;
        public DateTime FechaAdquisicion { get; set; }
        public string Estado { get; set; } = "Disponible";
        public string Imagen { get; set; } = string.Empty;
    }
}
