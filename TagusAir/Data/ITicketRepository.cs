using TagusAir.Data.Entities;

namespace TagusAir.Data
{
    public interface ITicketRepository :IGenericRepository<Ticket>
    {

        Task<List<Seat>> GetAvailableSeatsAsync(int flightId);

        Task<bool> IsSeatAvailableAsync(int flightId, int seatId);

        Task<Ticket> BuyTicketsAsync(int flightId, int seatId, string userId);

        Task<List<Ticket>> GetTicketsByUserAsync(string userId);


    }
}
