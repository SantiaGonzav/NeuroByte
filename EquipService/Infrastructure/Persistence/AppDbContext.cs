using EquipService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace EquipService.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<EquipoMedico> EquiposMedicos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EquipoMedico>()
                .Property(e => e.FechaAdquisicion)
                .HasColumnType("timestamp without time zone");

            // ✅ Conversor que asegura compatibilidad entre texto en DB y enum
            var estadoConverter = new ValueConverter<EquipoEstado, string>(
                v => v.ToString(),   // C# -> DB
                v => ConvertirEstado(v) // DB -> C#
            );

            modelBuilder.Entity<EquipoMedico>()
                .Property(e => e.Estado)
                .HasConversion(estadoConverter)
                .HasMaxLength(50);
        }

        // ✅ Método de conversión robusto y limpio
        private static EquipoEstado ConvertirEstado(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
                return EquipoEstado.Disponible;

            var normalized = v.Replace(" ", "").Trim().ToLowerInvariant();

            return normalized switch
            {
                "disponible" => EquipoEstado.Disponible,
                "ocupado" => EquipoEstado.Ocupado,
                "mantenimiento" or "mant" => EquipoEstado.Mantenimiento,
                _ => EquipoEstado.Disponible
            };
        }
    }
}
