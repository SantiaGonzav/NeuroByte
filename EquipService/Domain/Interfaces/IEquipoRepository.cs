using EquipService.Domain.Entities;

namespace EquipService.Domain.Interfaces
{
    public interface IEquipoRepository
    {
        Task<IEnumerable<EquipoMedico>> GetAllAsync();
        Task<EquipoMedico?> GetByIdAsync(int id);
        Task AddAsync(EquipoMedico equipo);
        Task UpdateAsync(EquipoMedico equipo);
        Task DeleteAsync(int id);
    }
}
