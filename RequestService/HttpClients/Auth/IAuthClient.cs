using RequestService.DTOs;

namespace RequestService.HttpClients.Auth
{
    public interface IAuthClient
    {
        Task<List<AdminUserDto>> GetAdministradoresAsync();
        Task<UsuarioDto?> GetUsuarioPorId(int id);
    }
}
