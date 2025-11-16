using System.Net.Http.Json;
using RequestService.DTOs;

namespace RequestService.HttpClients.Auth
{
    public class AuthClient : IAuthClient
    {
        private readonly HttpClient _http;

        public AuthClient(HttpClient http)
        {
            _http = http;
        }

        // ============================================================
        //  OBTENER LISTA DE ADMINISTRADORES
        // ============================================================
        public async Task<List<AdminUserDto>> GetAdministradoresAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<AdminUserDto>>("api/auth/admins");
                return result ?? new List<AdminUserDto>();
            }
            catch
            {
                return new List<AdminUserDto>();
            }
        }

        // ============================================================
        //  OBTENER UN USUARIO POR ID (ID = ENTERO)
        // ============================================================
        public async Task<UsuarioDto?> GetUsuarioPorId(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<UsuarioDto>($"api/auth/user/{id}");
            }
            catch
            {
                return null;
            }
        }
    }
}
