using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WeatherApp.API.Infrastructure;

public class JwtReader : IJwtReader
{
    public Claim? ReadToken(string authHeader, string claimName)
    {
        var token = authHeader.Substring("Bearer ".Length).Trim();
        var jwtHanlder = new JwtSecurityTokenHandler();
        var jwtToken = jwtHanlder.ReadJwtToken(token);

        return jwtToken.Claims.FirstOrDefault(c => c.Type == claimName);
    }   
}