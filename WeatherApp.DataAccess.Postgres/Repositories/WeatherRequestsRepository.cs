using Microsoft.EntityFrameworkCore;
using WeatherApp.DataAccess.Postgres.Interfaces;
using WeatherApp.DataAccess.Postgres.Models;

namespace WeatherApp.DataAccess.Postgres.Repositories;

public class WeatherRequestsRepository : IWeatherRequestsRepository
{
    private readonly WeatherDbContext _dbContext;
    public WeatherRequestsRepository(WeatherDbContext dbContext)
    {
        _dbContext = dbContext;
    }
   
    public async Task<List<WeatherRequest>> GetByUserId(Guid userId)
    {
        return await _dbContext.WeatherRequests
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .ToListAsync();
    }
    public async Task<List<WeatherRequest>> Get()
    {
        return await _dbContext.WeatherRequests
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<WeatherRequest>> GetMostPopular(int count)
    {
        var popularLocations = await _dbContext.WeatherRequests
            .GroupBy(r => r)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.Key)
            .ToListAsync();

        return popularLocations;
    }

    public async Task RemoveByUserId(Guid userId)
    {
        await _dbContext.WeatherRequests
            .Where(s => s.UserId == userId)
            .ExecuteDeleteAsync();
    }
    public async Task Add(Guid userId, Guid id, string country, string city, DateTime requestTime)
    {
        var searchRequest = new WeatherRequest()
        {
            Id = id,
            Country = country,
            City = city,
            RequestTime = requestTime,
            UserId = userId
        };

        await _dbContext.WeatherRequests.AddAsync(searchRequest);

        await _dbContext.SaveChangesAsync();
    }
}


