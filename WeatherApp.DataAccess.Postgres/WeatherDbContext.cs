using Microsoft.EntityFrameworkCore;
using WeatherApp.DataAccess.Postgres.Configurations;
using WeatherApp.DataAccess.Postgres.Models;

namespace WeatherApp.DataAccess.Postgres;
public class WeatherDbContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<WeatherRequest> WeatherRequests { get; set; }
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) :base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SearchRequestConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}