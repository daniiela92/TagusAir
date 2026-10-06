using Microsoft.AspNetCore.Mvc.Rendering;
using TagusAir.Data.Entities;

namespace TagusAir.Data
{
    public interface IFlightRepository : IGenericRepository<Flight>
    {
        IQueryable<Flight> GetAllWithDetails();

        Task<Flight> GetFlightWithDetailsAsync(int id);

        IQueryable<Flight> GetAvailableFlights();

        IQueryable<Flight> SearchFlights(DateTime? date, int? originAirportId);

        IEnumerable<SelectListItem> GetComboAirplanes();

        IEnumerable<SelectListItem> GetComboAirports();



    }
}
