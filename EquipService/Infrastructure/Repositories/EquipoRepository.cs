using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using EquipService.Domain.Entities;
using EquipService.Domain.Interfaces;
using EquipService.Infrastructure.Persistence;

namespace EquipService.Infrastructure.Repositories

{
    public class EquipoRepository : IEquipoRepository
    {
        private readonly AppDbContext _context;

        public EquipoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EquipoMedico>> GetAllAsync()
            => await _context.EquiposMedicos.ToListAsync();

        public async Task<EquipoMedico?> GetByIdAsync(int id)
            => await _context.EquiposMedicos.FindAsync(id);

        public async Task AddAsync(EquipoMedico equipo)
        {
            _context.EquiposMedicos.Add(equipo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EquipoMedico equipo)
        {
            _context.EquiposMedicos.Update(equipo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var equipo = await _context.EquiposMedicos.FindAsync(id);
            if (equipo != null)
            {
                _context.EquiposMedicos.Remove(equipo);
                await _context.SaveChangesAsync();
            }
        }
    }

}
