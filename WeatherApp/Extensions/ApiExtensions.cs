using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WeatherApp.API.Infrastructure;
using WeatherApp.API.Middlewares;
namespace WeatherApp.API.ApiExtensions
{
    public static class ApiExtensions
    {
        public static void AddApiAuthentication
            (
            this IServiceCollection services, 
            IConfiguration configuration
            ) 
        {
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.SecretKey)),
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
                    };

                    //options.Events = new JwtBearerEvents
                    //{
                    //    OnMessageReceived = context => 
                    //    {
                    //        context.Token = context.Request.Cookies["cookies-jwt"];

                    //        return Task.CompletedTask;
                    //    }
                    //}; Для кука, устанавливает в свойство токен из куки.
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                {
                    policy.RequireClaim("Admin", "true");
                });
            });
        }
    }
}
