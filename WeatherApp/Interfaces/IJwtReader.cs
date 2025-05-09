using System.Security.Claims;

namespace WeatherApp.API.Infrastructure;

public interface IJwtReader
{
    Claim ReadToken(string authHeader, string claimName);
}