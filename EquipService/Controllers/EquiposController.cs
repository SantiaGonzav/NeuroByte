using Microsoft.AspNetCore.Mvc;
using EquipService.Infrastructure.Persistence;
using EquipService.Domain.Entities;

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

        // ✅ GET: obtener todos los equipos
        [HttpGet]
        public IActionResult GetEquipos()
        {
            return Ok(_context.EquiposMedicos.ToList());
        }

        // ✅ GET by ID: obtener un equipo específico
        [HttpGet("{id}")]
        public IActionResult GetEquipoPorId(int id)
        {
            var equipo = _context.EquiposMedicos.Find(id);
            if (equipo == null)
                return NotFound($"No se encontró un equipo con ID {id}");

            return Ok(equipo);
        }

        // ✅ POST: crear un nuevo equipo
        [HttpPost]
        public IActionResult CrearEquipo([FromBody] EquipoMedico nuevoEquipo)
        {
            if (nuevoEquipo == null)
                return BadRequest("Los datos del equipo no pueden estar vacíos.");

            _context.EquiposMedicos.Add(nuevoEquipo);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetEquipoPorId), new { id = nuevoEquipo.Id }, nuevoEquipo);
        }

        // ✅ PUT: actualizar un equipo existente
        [HttpPut("{id}")]
        public IActionResult ActualizarEquipo(int id, [FromBody] EquipoMedico equipoActualizado)
        {
            var equipo = _context.EquiposMedicos.Find(id);
            if (equipo == null)
                return NotFound($"No se encontró un equipo con ID {id}");

            // Actualizar campos
            equipo.Nombre = equipoActualizado.Nombre;
            equipo.NumeroSerie = equipoActualizado.NumeroSerie;
            equipo.Modelo = equipoActualizado.Modelo;
            equipo.FechaAdquisicion = equipoActualizado.FechaAdquisicion;
            equipo.Estado = equipoActualizado.Estado;
            equipo.Imagen = equipoActualizado.Imagen;





            _context.SaveChanges();
            return Ok(equipo);
        }

        // ✅ DELETE: eliminar un equipo por ID
        [HttpDelete("{id}")]
        public IActionResult EliminarEquipo(int id)
        {
            var equipo = _context.EquiposMedicos.Find(id);
            if (equipo == null)
                return NotFound($"No se encontró un equipo con ID {id}");

            _context.EquiposMedicos.Remove(equipo);
            _context.SaveChanges();

            return NoContent(); // 204: Eliminado correctamente sin contenido
        }
    }
}
