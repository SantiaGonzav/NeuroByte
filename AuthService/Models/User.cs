using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }   // ← INT AUTOINCREMENTAL

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "user";

        public string? ProfileImagePath { get; set; }
    }
}
