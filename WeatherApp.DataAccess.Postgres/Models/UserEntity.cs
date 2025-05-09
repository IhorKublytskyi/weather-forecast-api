namespace WeatherApp.DataAccess.Postgres.Models;
public class UserEntity
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public IList<WeatherRequest> SearchRequests  { get; set; } = new List<WeatherRequest>(); // Связь один ко многим
}