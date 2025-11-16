using AuthService.Auth;
using AuthService.Data;
using AuthService.Models;
using AuthService.Models.Dtos;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwt;
        private readonly EmailService _emailService;

        public AuthController(AppDbContext context, JwtService jwt, EmailService emailService)
        {
            _context = context;
            _jwt = jwt;
            _emailService = emailService;
        }

        // ============================================================
        //  REGISTRO
        // ============================================================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Todos los campos son obligatorios.");

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("El correo ya está registrado.");

            using var sha = SHA256.Create();
            var hashedPassword = Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(request.Password))
            );

            string role = string.IsNullOrWhiteSpace(request.Role)
                ? "user"
                : request.Role.Trim().ToLower();

            var user = new User
            {
                Username = string.IsNullOrWhiteSpace(request.Username)
                    ? request.Email.Split('@')[0]
                    : request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = role
            };

            if (request.ProfileImage != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ProfileImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await request.ProfileImage.CopyToAsync(stream);

                user.ProfileImagePath = $"/images/profiles/{fileName}";
            }
            else
            {
                user.ProfileImagePath = "/images/defaults/default-profile.png";
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado correctamente ✅" });
        }

        // ============================================================
        //  LOGIN
        // ============================================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Correo y contraseña son obligatorios.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized("Correo no registrado.");

            using var sha = SHA256.Create();
            var hashedInput = Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(request.Password))
            );

            if (user.PasswordHash != hashedInput)
                return Unauthorized("Contraseña incorrecta.");

            var token = _jwt.GenerateToken(user);

            return Ok(new
            {
                message = "Inicio de sesión exitoso.",
                token,
                user = new UsuarioDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role.ToLower(),
                    ProfileImagePath = user.ProfileImagePath
                }
            });
        }

        // ============================================================
        //  RECUPERAR CONTRASEÑA
        // ============================================================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("El correo es obligatorio.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return BadRequest("Correo no registrado.");

            var newPassword = Guid.NewGuid().ToString("N")[..8];

            using var sha = SHA256.Create();
            user.PasswordHash = Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(newPassword))
            );

            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email,
                "🔐 Recuperación de contraseña - NeuroByte",
                $@"
                    <h2>Hola {user.Username},</h2>
                    <p>Tu nueva contraseña temporal es:</p>
                    <p><b>{newPassword}</b></p>
                    <p>Por favor cámbiala al iniciar sesión.</p>"
            );

            return Ok("Se envió una nueva contraseña temporal al correo registrado.");
        }

        // ============================================================
        //  LISTA DE ADMINS
        // ============================================================
        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await _context.Users
                .Where(u => u.Role.ToLower() == "admin")
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    ProfileImagePath = u.ProfileImagePath
                })
                .ToListAsync();

            return Ok(admins);
        }

        // ============================================================
        //  OBTENER USUARIO POR ID (INT)
        // ============================================================
        [HttpGet("user/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("Usuario no encontrado.");

            return Ok(new UsuarioDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ProfileImagePath = user.ProfileImagePath
            });
        }
    }
}
