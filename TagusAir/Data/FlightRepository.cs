using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TagusAir.Data.Entities;

namespace TagusAir.Data
{
    public class FlightRepository : GenericRepository<Flight>, IFlightRepository
    {
        private readonly DataContext _context;

        public FlightRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Flight> GetAllWithDetails()
        {
            return _context.Flights
                .Include(f => f.DepartureAirport)
                .ThenInclude(a => a.Country)
                .Include(f => f.ArrivalAirport)
                .ThenInclude(a => a.Country)
                .Include(f => f.Airplane)
                .OrderBy(f => f.DepartureTime);
        }

        public async Task<Flight> GetFlightWithDetailsAsync(int id)
        {
            return await _context.Flights
                .Include(f => f.DepartureAirport)
                .ThenInclude(a => a.Country)
                .Include(f => f.ArrivalAirport)
                .ThenInclude(a => a.Country)
                .Include(f => f.Airplane)
                .Where(f => f.Id == id)
                .FirstOrDefaultAsync();
        }

        public IQueryable<Flight> GetAvailableFlights()
        {
            return this.GetAllWithDetails()
                .Where(f => f.DepartureTime > DateTime.Now);
        }

        public IQueryable<Flight> SearchFlights(DateTime? date, int? originAirportId)
        {
            var flights = this.GetAvailableFlights();

            if (date.HasValue)
            {
                flights = flights.Where(f => f.DepartureTime.Date == date.Value.Date);
            }

            if (originAirportId.HasValue && originAirportId.Value != 0)
            {
                flights = flights.Where(f => f.DepartureAirportId == originAirportId.Value);
            }

            return flights;
        }

        public IEnumerable<SelectListItem> GetComboAirplanes()
        {
            var list = _context.Airplanes
                .Where(a => a.IsActive)
                .ToList()
                .Select(a => new SelectListItem
                {
                    Text = $"{a.Brand} {a.Model}",
                    Value = a.Id.ToString()
                })
                .OrderBy(l => l.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select an airplane...)",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboAirports()
        {
            var list = _context.Airports
                .ToList()
                .Select(a => new SelectListItem
                {
                    Text = $"{a.City} - {a.Name} ({a.IataCode})",
                    Value = a.Id.ToString()
                })
                .OrderBy(l => l.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select an airport...)",
                Value = "0"
            });

            return list;
        }
    }
}
