namespace WeatherApp.API.Services.Models;

public record RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}