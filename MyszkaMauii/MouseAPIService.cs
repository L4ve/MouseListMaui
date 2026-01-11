using System.Net.Http.Json;

namespace MyszkaMauii
{
    public class MouseApiService
    {
        private readonly HttpClient _httpClient;

        public MouseApiService()
        {
            _httpClient = new HttpClient
            {
                // localhost do skopiowania z api/properties/launchsettings.json
                BaseAddress = new Uri("http://localhost:5231/api/")
            };
        }

        public async Task<List<Mouse1>> GetMiceAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Mouse1>>("mice")
                   ?? new List<Mouse1>();
        }

        public async Task<Mouse1> AddMouseAsync(Mouse1 mouse)
        {
            var response = await _httpClient.PostAsJsonAsync("mice", mouse);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }

            // api zwraca nową mysz z id
            return await response.Content.ReadFromJsonAsync<Mouse1>();
        }
    }
}
