using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EquipService.Infrastructure.Persistence;
using EquipService.Domain.Entities;
using EquipService.Application.DTOs;

namespace EquipService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquiposController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EquiposController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Obtener todos los equipos
        [HttpGet]
        public async Task<IActionResult> GetEquipos()
        {
            var equipos = await _context.EquiposMedicos.AsNoTracking().ToListAsync();
            return Ok(equipos);
        }

        // ✅ Obtener un equipo por ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEquipoPorId(int id)
        {
            var equipo = await _context.EquiposMedicos.FindAsync(id);
            if (equipo == null)
                return NotFound($"No se encontró un equipo con ID {id}");

            return Ok(equipo);
        }

        // ✅ Crear un nuevo equipo
        [HttpPost]
        public async Task<IActionResult> CrearEquipo([FromBody] EquipoMedicoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var nuevoEquipo = new EquipoMedico
            {
                Nombre = dto.Nombre,
                Modelo = dto.Modelo,
                NumeroSerie = dto.NumeroSerie,
                FechaAdquisicion = dto.FechaAdquisicion,
                Imagen = dto.Imagen,
                Estado = ParseEstado(dto.Estado)
            };

            _context.EquiposMedicos.Add(nuevoEquipo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEquipoPorId), new { id = nuevoEquipo.Id }, nuevoEquipo);
        }

        // ✅ Actualizar equipo existente
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarEquipo(int id, [FromBody] EquipoMedicoDto dto)
        {
            var equipo = await _context.EquiposMedicos.FindAsync(id);
            if (equipo == null)
                return NotFound($"No se encontró un equipo con ID {id}");

            equipo.Nombre = dto.Nombre;
            equipo.Modelo = dto.Modelo;
            equipo.NumeroSerie = dto.NumeroSerie;
            equipo.FechaAdquisicion = dto.FechaAdquisicion;
            equipo.Imagen = dto.Imagen;
            equipo.Estado = ParseEstado(dto.Estado);

            await _context.SaveChangesAsync();
            return Ok(equipo);
        }

        // ✅ Eliminar equipo
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarEquipo(int id)
        {
            var equipo = await _context.EquiposMedicos.FindAsync(id);
            if (equipo == null)
                return NotFound($"No se encontró un equipo con ID {id}");

            _context.EquiposMedicos.Remove(equipo);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 🔧 Método auxiliar para convertir texto → enum
        private static EquipoEstado ParseEstado(string estadoTexto)
        {
            if (string.IsNullOrWhiteSpace(estadoTexto))
                return EquipoEstado.Disponible;

            return estadoTexto.Trim().ToLowerInvariant() switch
            {
                "disponible" => EquipoEstado.Disponible,
                "ocupado" => EquipoEstado.Ocupado,
                "mantenimiento" => EquipoEstado.Mantenimiento,
                _ => EquipoEstado.Disponible
            };
        }
    }
}
