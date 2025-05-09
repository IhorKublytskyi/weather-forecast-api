using System.Text.Json.Serialization;

namespace WeatherApp.DataAccess.Postgres.Models;

public class WeatherRequest
{
    public Guid Id { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime RequestTime { get; set; }
    public Guid UserId { get; set; } //Foreign Key
    [JsonIgnore]
    public UserEntity? User { get; set; }
}