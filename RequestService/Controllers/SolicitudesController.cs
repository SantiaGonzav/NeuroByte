using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestService.Data;
using RequestService.DTOs;
using RequestService.HttpClients;
using RequestService.HttpClients.Auth;
using RequestService.Models;
using RequestService.Services;
using System.Security.Claims;

namespace RequestService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SolicitudesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEquipClient _equipClient;
        private readonly IAuthClient _authClient;
        private readonly IEmailService _emailService;

        public SolicitudesController(
            ApplicationDbContext context,
            IEquipClient equipClient,
            IAuthClient authClient,
            IEmailService emailService)
        {
            _context = context;
            _equipClient = equipClient;
            _authClient = authClient;
            _emailService = emailService;
        }

        // ============================================================
        // CREAR SOLICITUD
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> CrearSolicitud([FromBody] CrearSolicitudDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdStr == null)
                return Unauthorized("Token inválido.");

            int userId = int.Parse(userIdStr);

            var solicitud = new Solicitud
            {
                UserId = userId,
                EquipoId = dto.EquipoId,
                Procedimiento = dto.Procedimiento,
                Observaciones = dto.Observaciones,
                FechaProgramada = dto.FechaProgramada.ToUniversalTime(),
                Estado = "Pendiente"
            };

            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSolicitudPorId), new { id = solicitud.Id }, solicitud);
        }

        // ============================================================
        // MIS SOLICITUDES (USUARIO NORMAL)
        // ============================================================
        [HttpGet("mias")]
        public async Task<IActionResult> GetMisSolicitudes()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null)
                return Unauthorized("Token inválido.");

            int userId = int.Parse(userIdStr);

            var solicitudes = await _context.Solicitudes
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();

            var resultado = new List<SolicitudDetalleDto>();

            foreach (var s in solicitudes)
            {
                var equipo = await _equipClient.GetEquipoAsync(s.EquipoId);

                UsuarioDto? revisor = null;
                if (s.ReviewedById.HasValue)
                    revisor = await _authClient.GetUsuarioPorId(s.ReviewedById.Value);

                resultado.Add(new SolicitudDetalleDto
                {
                    Id = s.Id,
                    FechaSolicitud = s.FechaSolicitud,
                    FechaProgramada = s.FechaProgramada,
                    Procedimiento = s.Procedimiento,
                    Observaciones = s.Observaciones,
                    Estado = s.Estado,
                    EquipoId = s.EquipoId,
                    Equipo = equipo,
                    Usuario = null,
                    RevisadoPorUsuario = revisor
                });
            }

            return Ok(resultado);
        }

        // ============================================================
        // LISTAR TODAS (ADMIN)
        // ============================================================
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetSolicitudes()
        {
            var solicitudes = await _context.Solicitudes
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();

            var result = new List<SolicitudDetalleDto>();

            foreach (var s in solicitudes)
            {
                var equipo = await _equipClient.GetEquipoAsync(s.EquipoId);
                var usuario = await _authClient.GetUsuarioPorId(s.UserId);

                UsuarioDto? revisor = null;
                if (s.ReviewedById.HasValue)
                    revisor = await _authClient.GetUsuarioPorId(s.ReviewedById.Value);

                result.Add(new SolicitudDetalleDto
                {
                    Id = s.Id,
                    FechaSolicitud = s.FechaSolicitud,
                    FechaProgramada = s.FechaProgramada,
                    Procedimiento = s.Procedimiento,
                    Observaciones = s.Observaciones,
                    Estado = s.Estado,
                    EquipoId = s.EquipoId,
                    Equipo = equipo,
                    Usuario = usuario,
                    RevisadoPorUsuario = revisor
                });
            }

            return Ok(result);
        }

        // ============================================================
        // APROBAR
        // ============================================================
        [HttpPut("{id:int}/aprobar")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AprobarSolicitud(int id)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null)
                return NotFound("Solicitud no encontrada.");

            var reviewerStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (reviewerStr == null)
                return Unauthorized();

            solicitud.Estado = "Aprobada";
            solicitud.ReviewedById = int.Parse(reviewerStr);
            solicitud.FechaRevision = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(solicitud);
        }

        // ============================================================
        // RECHAZAR
        // ============================================================
        [HttpPut("{id:int}/rechazar")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RechazarSolicitud(int id)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null)
                return NotFound("Solicitud no encontrada.");

            var reviewerStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (reviewerStr == null)
                return Unauthorized();

            solicitud.Estado = "Rechazada";
            solicitud.ReviewedById = int.Parse(reviewerStr);
            solicitud.FechaRevision = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(solicitud);
        }

        // ============================================================
        // OBTENER POR ID
        // ============================================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSolicitudPorId(int id)
        {
            var s = await _context.Solicitudes.FindAsync(id);
            if (s == null)
                return NotFound("Solicitud no encontrada.");

            var equipo = await _equipClient.GetEquipoAsync(s.EquipoId);
            var usuario = await _authClient.GetUsuarioPorId(s.UserId);

            UsuarioDto? revisor = null;
            if (s.ReviewedById.HasValue)
                revisor = await _authClient.GetUsuarioPorId(s.ReviewedById.Value);

            return Ok(new SolicitudDetalleDto
            {
                Id = s.Id,
                FechaSolicitud = s.FechaSolicitud,
                FechaProgramada = s.FechaProgramada,
                Procedimiento = s.Procedimiento,
                Observaciones = s.Observaciones,
                Estado = s.Estado,
                EquipoId = s.EquipoId,
                Equipo = equipo,
                Usuario = usuario,
                RevisadoPorUsuario = revisor
            });
        }
    }
}
