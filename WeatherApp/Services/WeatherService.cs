using WeatherApp.Services.Models;

namespace WeatherApp.API.Services
{
    class WeatherService
    {
        private readonly HttpClient _httpClient;
        public WeatherService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WeatherAPI");
        }

        public async Task<WeatherResponse?> GetWeatherAsync(string apiCallString)
        {
            var response = await _httpClient.GetAsync(apiCallString);

            return await response.Content.ReadFromJsonAsync<WeatherResponse>();
        }
    }
}
