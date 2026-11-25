using Microsoft.AspNetCore.Mvc.Rendering;
using TagusAir.Data.Entities;
using TagusAir.Helpers;
using TagusAir.Models;

namespace TagusAir.Data
{
    public interface ICountryRepository : IGenericRepository<Country>
    {

        IQueryable GetCountriesWithAirports();

        Task<Country> GetCountryWithAirportsAsync(int id);

        Task<Airport> GetAirportAsync(int id);

        Task AddAirportAsync(AirportViewModel model, AirportLocation location);

        Task<int> UpdateAirportAsync(Airport airport);

        Task<int> DeleteAirportAsync(Airport airport);

        IEnumerable<SelectListItem> GetComboCountries();

        IEnumerable<SelectListItem> GetComboAirports(int countryId);

        Task<bool> CountryHasAirportsAsync(int countryId);

        Task<bool> AirportHasFlightsAsync(int airportId);

    }
}
