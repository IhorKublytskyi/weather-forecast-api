using WeatherApp.DataAccess.Postgres.Models;

namespace WeatherApp.DataAccess.Postgres.Interfaces
{
    public interface IWeatherRequestsRepository
    {
        Task Add(Guid userId, Guid id, string country, string city, DateTime requestTime);
        Task<List<WeatherRequest>> GetByUserId(Guid userId);
        Task<List<WeatherRequest>> Get();
        Task<List<WeatherRequest>> GetMostPopular(int count);
        Task RemoveByUserId(Guid userId);
    }
}