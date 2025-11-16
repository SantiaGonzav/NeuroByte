using Microsoft.AspNetCore.Http;

namespace AuthService.Models
{
    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // 👑 Nuevo campo para definir el rol del usuario (por defecto "user")
        public string Role { get; set; } = "user";

        // 🖼️ Imagen opcional de perfil
        public IFormFile? ProfileImage { get; set; }
    }
}
