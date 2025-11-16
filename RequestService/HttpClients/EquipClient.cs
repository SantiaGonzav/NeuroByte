namespace RequestService.HttpClients
{
    public interface IEquipClient
    {
        Task<EquipoDto?> GetEquipoAsync(int equipoId);
    }

    public class EquipClient : IEquipClient
    {
        private readonly HttpClient _httpClient;

        public EquipClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EquipoDto?> GetEquipoAsync(int equipoId)
        {
            var response = await _httpClient.GetAsync($"api/equipos/{equipoId}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<EquipoDto>();
        }
    }
}
