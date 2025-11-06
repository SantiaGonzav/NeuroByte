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

        // ✅ Registro de usuario
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.PasswordHash))
                return BadRequest("Todos los campos son obligatorios.");

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("El correo ya está registrado.");

            using var sha = SHA256.Create();
            var hashedPassword = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(request.PasswordHash)));

            var user = new User
            {
                Username = request.Username ?? request.Email.Split('@')[0], // Si no se envía username, lo genera a partir del email
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = string.IsNullOrEmpty(request.Role) ? "User" : request.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado correctamente ✅");
        }

        // ✅ Inicio de sesión (solo por correo)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.PasswordHash))
                return BadRequest("Correo y contraseña son obligatorios.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized("Correo no registrado.");

            using var sha = SHA256.Create();
            var hashedInput = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(request.PasswordHash)));

            if (user.PasswordHash != hashedInput)
                return Unauthorized("Contraseña incorrecta.");

            var token = _jwt.GenerateToken(user);

            return Ok(new
            {
                message = "Inicio de sesión exitoso.",
                token,
                user = new { user.Username, user.Email, user.Role }
            });
        }

        // ✅ Recuperación de contraseña
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("El correo es obligatorio.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return BadRequest("Correo no registrado.");

            // Generar una nueva contraseña temporal (8 caracteres)
            var newPassword = Guid.NewGuid().ToString("N")[..8];

            using var sha = SHA256.Create();
            user.PasswordHash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(newPassword)));

            await _context.SaveChangesAsync();

            // 🧩 Línea de depuración
            Console.WriteLine($"[DEBUG] Enviando correo de recuperación a: {user.Email}");

            // Enviar correo
            await _emailService.SendEmailAsync(
                user.Email,
                "🔐 Recuperación de contraseña - NeuroByte",
                $@"
                <h2>Hola {user.Username},</h2>
                <p>Tu nueva contraseña temporal es:</p>
                <p><b>{newPassword}</b></p>
                <p>Por favor cámbiala al iniciar sesión.</p>
                <br/>
                <p>Atentamente,<br/>El equipo de <b>NeuroByte</b></p>"
            );

            return Ok("Se envió una nueva contraseña temporal al correo registrado.");
        }
    }
}
