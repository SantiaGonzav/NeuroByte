using EquipService.Domain.Entities;
using EquipService.Domain.Interfaces;

namespace EquipService.Application.Services
{
    public class EquipoService
    {
        private readonly IEquipoRepository _repository;

        public EquipoService(IEquipoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EquipoMedico>> ListarEquiposAsync()
            => await _repository.GetAllAsync();
    }

}
