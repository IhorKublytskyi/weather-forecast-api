using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherApp.DataAccess.Postgres.Models;

namespace WeatherApp.DataAccess.Postgres.Configurations;
public class SearchRequestConfiguration : IEntityTypeConfiguration<WeatherRequest>
{
    public void Configure(EntityTypeBuilder<WeatherRequest> builder)
    {
        builder.HasKey(r => r.Id);

        builder
            .HasOne(r => r.User)
            .WithMany(u => u.SearchRequests)
            .HasForeignKey(r => r.UserId);
    }
}