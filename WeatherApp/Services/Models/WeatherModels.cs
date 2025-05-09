using System.Text.Json.Serialization;

namespace WeatherApp.Services.Models
{
    class WeatherResponse
    {
        [JsonPropertyName("main")]
        public MainData? Main { get; set; }
        [JsonPropertyName("wind")]
        public WindData? Wind { get; set; }
    }
    class MainData
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }
        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }
        [JsonPropertyName("pressure")]
        public double Pressure { get; set; }
        [JsonPropertyName("humidity")]
        public double Humidity { get; set; }

    }
    class WindData
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }
    }
}
