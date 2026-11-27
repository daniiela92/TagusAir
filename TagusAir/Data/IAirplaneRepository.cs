using TagusAir.Data.Entities;

namespace TagusAir.Data
{
    public interface IAirplaneRepository : IGenericRepository<Airplane>
    {
        Task CreateWithSeatsAsync(Airplane airplane);

        Task<List<Ticket>> UpdateWithSeatsAsync(Airplane airplane);

        Task DeleteWithSeatsAsync(Airplane airplane);

        Task<bool> AirplaneHasFlightsAsync(int airplaneId);

    }
}
