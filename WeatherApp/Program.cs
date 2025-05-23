using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WeatherApp.API.ApiExtensions;
using WeatherApp.API.Infrastructure;
using WeatherApp.API.Middlewares;
using WeatherApp.API.Services;
using WeatherApp.API.Services.Models;
using WeatherApp.DataAccess.Postgres;
using WeatherApp.DataAccess.Postgres.Interfaces;
using WeatherApp.DataAccess.Postgres.Models;
using WeatherApp.DataAccess.Postgres.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WeatherDbContext>(options => 
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(WeatherDbContext)));
});
builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

builder.Services.AddScoped<JwtOptions>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IJwtReader, JwtReader>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<IWeatherRequestsRepository, WeatherRequestsRepository>();
builder.Services.AddSingleton<ICountryCodeValidator, CountryCodeValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("WeatherAPI", client => 
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
});

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Weather API", Version = "v1" });
    
    // JWT-авторизация Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT-токен"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseExceptionHandler(app => app.UseMiddleware<ExceptionHandlingMiddleware>());
//app.UseCookiePolicy(new CookiePolicyOptions 
//{
//    MinimumSameSitePolicy = SameSiteMode.Strict,
//    HttpOnly = HttpOnlyPolicy.Always,
//    Secure = CookieSecurePolicy.Always
//}); Настройки для куки, если добавлять токен в куки.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseStaticFiles();
//app.UseDefaultFiles();

/*app.MapGet("/", async context => 
{
    context.Response.Headers.ContentType = "text/html; charset=utf8";

    await context.Response.SendFileAsync("C:\\Users\\igorl\\OneDrive\\Desktop\\WeatherApp\\WeatherApp\\wwwroot\\index.html");
});

app.MapGet("/weather.html", async (context) => 
{
    context.Response.Headers.ContentType = "text/html; charset=utf8";

    await context.Response.SendFileAsync("C:\\Users\\igorl\\OneDrive\\Desktop\\WeatherApp\\WeatherApp\\wwwroot\\weather.html");
});*/

app.Map("/", async context =>
{
    context.Response.Redirect("/swagger");
});
app.MapGet("/weather/{country:required}/{city:required}", [Authorize] async (HttpContext context, 
    IUsersRepository usersRepository, 
    IWeatherRequestsRepository requestsRepository, 
    IConfiguration config, 
    WeatherService weatherService,
    IJwtReader jwtReader,
    ICountryCodeValidator countryCodeValidator,
    string country, 
    string city) => 
{
    if (string.IsNullOrWhiteSpace(country))
        return Results.BadRequest();
    if (string.IsNullOrWhiteSpace(city))
        return Results.BadRequest();
    if (!countryCodeValidator.IsCountryCodeValid(country))
        return Results.BadRequest("Invalid country code.");

    var id = jwtReader.ReadToken(context.Request.Headers.Authorization.ToString(), "Id")?.Value;
    var user =  await usersRepository.GetById(Guid.Parse(id));

    await requestsRepository.Add(user.Id, new Guid(), country, city, DateTime.UtcNow);

    string? apiKey = config["OpenWeatherMap:ApiKey"];
    string apiCallString = $"weather?q={city}, {country}&appid={apiKey}&units=metric&lang=ru";

    var data = await weatherService.GetWeatherAsync(apiCallString);

    if (data.Main != null && data.Wind != null)
    {
        return Results.Json(new
        {
            Temp = data.Main.Temp,
            FeelsLike = data.Main.FeelsLike,
            Humidity = data.Main.Humidity,
            Speed = data.Wind.Speed,
            Pressure = data.Main.Pressure
        });
    }
    else
    {
        return Results.Problem(detail: "Not found", statusCode: 502);
    }
});
app.MapGet("/history", [Authorize] async (HttpContext context, 
    IUsersRepository usersRepository, 
    IWeatherRequestsRepository requestsRepository,
    IJwtReader jwtReader) =>
{
    var id = jwtReader.ReadToken(context.Request.Headers.Authorization.ToString(), "Id")?.Value;

    var requests = await requestsRepository.GetByUserId(Guid.Parse(id));

    if (requests.Count == 0)
        return Results.Ok("Your search history is empty.");

    return Results.Ok(requests);
});
tsRapp.MapGet("/history/popular", [Authorize] async (IWeatherRequestsRepository requesepository) =>
{
    var popularRequests = await requestsRepository.GetMostPopular(3);

    return Results.Ok(popularRequests);
});
app.MapDelete("/history/delete", [Authorize] async (HttpContext context,
    IWeatherRequestsRepository requestsRepository,
    IJwtReader jwtReader) =>
{
    var id = jwtReader.ReadToken(context.Request.Headers.Authorization.ToString(), "Id")?.Value;

    await requestsRepository.RemoveByUserId(Guid.Parse(id));
});

app.MapPost("/register", async (RegisterRequest request, IUsersRepository usersRepository, IPasswordHasher hasher) =>
{
    if (!new EmailAddressAttribute().IsValid(request.Email))
        return Results.BadRequest("Invalid email format.");
    
    if (await usersRepository.Exists(request.Email))
    {
        return Results.BadRequest("User already exists.");
    }

    string hashedPassword = hasher.HashPassword(request.Password);

    await usersRepository.Add(new Guid(), request.Email, hashedPassword, DateTime.UtcNow);

    return Results.Ok("You have registered successful.");
});

app.MapPost("/login", async (LoginRequest loginRequest, 
    IUsersRepository repository, 
    IJwtProvider jwtProvider,
    IPasswordHasher hasher,
    HttpContext context,
    CancellationToken cancellationToken) =>
{
    UserEntity? user = await repository.GetByEmail(loginRequest.Email);

    if (user is null)
        return Results.BadRequest("User does not exist.");
    
    if (!hasher.VerifyHashedPassword(loginRequest.Password, user.HashedPassword))
        return Results.BadRequest("Password is incorrect.");
    
    var token = jwtProvider.GenerateToken(user);

    //context.Response.Cookies.Append("cookies-jwt", token); // Для куки

    return Results.Json(new
    {
        access_token = token,
        username = user.Email
    });
});

app.Run();