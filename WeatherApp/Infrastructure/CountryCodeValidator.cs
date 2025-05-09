using System.Globalization;

namespace WeatherApp.API.Infrastructure;

public class CountryCodeValidator : ICountryCodeValidator
{
    public bool IsCountryCodeValid(string countryCode)
    {
        return CultureInfo
            .GetCultures(CultureTypes.SpecificCultures)
            .Select(culture => new RegionInfo(culture.Name))
            .Any(region => region.TwoLetterISORegionName == countryCode);
    }
}