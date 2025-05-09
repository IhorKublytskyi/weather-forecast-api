using WeatherApp.DataAccess.Postgres.Models;

namespace WeatherApp.API.Infrastructure
{
    public interface IJwtProvider
    {
        string GenerateToken(UserEntity user);
    }
}