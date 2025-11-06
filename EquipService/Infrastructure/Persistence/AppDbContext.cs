using Microsoft.EntityFrameworkCore;
using EquipService.Domain.Entities;

namespace EquipService.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<EquipoMedico> EquiposMedicos { get; set; }
    }
}
