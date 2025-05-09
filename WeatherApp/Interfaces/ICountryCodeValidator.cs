namespace WeatherApp.API.Infrastructure;

public interface ICountryCodeValidator
{
    public bool IsCountryCodeValid(string countryCode);
}