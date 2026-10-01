using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TagusAir.Data.Entities;
using TagusAir.Models;

namespace TagusAir.Data
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        private readonly DataContext _context;

        public CountryRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddAirportAsync(AirportViewModel model)
        {
            var country = await this.GetCountryWithAirportsAsync(model.CountryId);

            if (country == null)
            {
                return;
            }

            country.Airports.Add(new Airport
            {
                Name = model.Name,
                City = model.City,
                IataCode = model.IataCode
            });

            _context.Countries.Update(country);
            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAirportAsync(Airport airport)
        {
            var country = await _context.Countries
                .Where(c => c.Airports.Any(a => a.Id == airport.Id))
                .FirstOrDefaultAsync();

            if (country == null)
            {
                return 0;
            }

            _context.Airports.Remove(airport);
            await _context.SaveChangesAsync();

            return country.Id;
        }

   

        public async Task<Airport> GetAirportAsync(int id)
        {
            return await _context.Airports.FindAsync(id);
        }

        public IEnumerable<SelectListItem> GetComboAirports(int countryId)
        {
            var country = _context.Countries.Find(countryId);
            var list = new List<SelectListItem>();

            if (country != null)
            {
                list = _context.Airports
                    .Where(a => a.CountryId == countryId)
                    .Select(a => new SelectListItem
                    {
                        Text = $"{a.City} - {a.Name} ({a.IataCode})",
                        Value = a.Id.ToString()
                    })
                    .OrderBy(l => l.Text)
                    .ToList();
            }

            list.Insert(0, new SelectListItem
            {
                Text = "(Select an airport...)",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboCountries()
        {
            var list = _context.Countries.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a country...)",
                Value = "0"
            });

            return list;
        }

        public IQueryable GetCountriesWithAirports()
        {
            return _context.Countries
                .Include(c => c.Airports)
                .OrderBy(c => c.Name);
        }

        public async Task<Country> GetCountryWithAirportsAsync(int id)
        {
            return await _context.Countries
              .Include(c => c.Airports)
              .Where(c => c.Id == id)
              .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAirportAsync(Airport airport)
        {
            var country = await _context.Countries
                .Where(c => c.Airports.Any(a => a.Id == airport.Id))
                .FirstOrDefaultAsync();

            if (country == null)
            {
                return 0;
            }

            _context.Airports.Update(airport);
            await _context.SaveChangesAsync();

            return country.Id;
        }
    }
}
