using WeatherApp.API.Infrastructure;
using WeatherApp.DataAccess.Postgres.Interfaces;

namespace WeatherApp.API.Endpoints;

public static class HistoryEndpoints
{
    public static RouteGroupBuilder MapHistoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/history").RequireAuthorization();

        group.MapGet("/", Get );
        group.MapDelete("/delete", Delete);

        return group;
    }

    private static async Task<IResult> Get
    (
        HttpContext context,
        IUsersRepository usersRepository,
        IWeatherRequestsRepository requestsRepository,
        IJwtReader jwtReader)
    {
        var id = jwtReader.ReadToken(context.Request.Headers.Authorization.ToString(), "Id")?.Value;

        if (id != null)
        {
            var requests = await requestsRepository.GetByUserId(Guid.Parse(id));

            return requests.Count == 0 ? Results.Ok("Your search history is empty.") : Results.Ok(requests);
        }

        return Results.BadRequest("User was not found.");
    }

    private static async Task<IResult> Delete(HttpContext context,
        IWeatherRequestsRepository requestsRepository,
        IJwtReader jwtReader)
    {
        var id = jwtReader.ReadToken(context.Request.Headers.Authorization.ToString(), "Id")?.Value;

        if (id != null) 
            await requestsRepository.RemoveByUserId(Guid.Parse(id));

        return Results.BadRequest("User was not found");
    }
}