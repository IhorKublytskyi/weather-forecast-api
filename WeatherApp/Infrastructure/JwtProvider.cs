using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherApp.DataAccess.Postgres.Models;

namespace WeatherApp.API.Infrastructure
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _options;
        public JwtProvider(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }
        public string GenerateToken(UserEntity user)
        {
            var claims = new Claim[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Admin", "true")
            };

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                    signingCredentials: signingCredentials,
                    expires: DateTime.UtcNow.AddHours(_options.Expires),
                    claims: claims
                );

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
    }
}
