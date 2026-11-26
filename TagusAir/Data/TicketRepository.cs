using Microsoft.EntityFrameworkCore;
using TagusAir.Data.Entities;
using TagusAir.Helpers;

namespace TagusAir.Data
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public TicketRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task<Ticket> BuyTicketsAsync(int flightId, int seatId, string userId)
        {
            var flight = await _context.Flights.FindAsync(flightId);

            var seat = await _context.Seats.FindAsync(seatId);

            if(flight == null || !flight.IsActive || seat == null || !seat.IsActive)
            {
                return null!; 
            }

            var ticket = new Ticket
            {
                FlightId = flight.Id,
                UserId = userId,
                SeatId = seatId,
                PurchaseDate = DateTime.Now,
                Price = seat.Class == SeatClass.Business ? flight.BusinessPrice : flight.BasePrice,
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

        public async Task<bool> FlightHasTicketsAsync(int flightId)
        {
            return await _context.Tickets.AnyAsync(t => t.FlightId == flightId);
        }

        public async Task<List<Seat>> GetAvailableSeatsAsync(int flightId)
        {
            var flight = await _context.Flights.FindAsync(flightId);

            if(flight == null)
            {
                return new List<Seat>();
            }

            var soldTickets = _context.Tickets
                .Where(t => t.FlightId == flightId)
                .Select(t => t.SeatId);

            return await _context.Seats
                 .Where(s => s.AirplaneId == flight.AirplaneId && s.IsActive && !soldTickets.Contains(s.Id))
                 .OrderBy(s => s.Class)
                 .ThenBy(s => s.Code)
                 .ToListAsync();




        }

        public async Task<IQueryable<Ticket>> GetTicketsAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return null!;
            }

            if(await _userHelper.IsUserInRoleAsync(user, "Employee")) 
            {
                return _context.Tickets
                    .Include(t => t.Seat)
                    .Include(t => t.Flight)
                    .ThenInclude(f => f.DepartureAirport)
                    .Include(t => t.Flight)
                    .ThenInclude(f => f.ArrivalAirport)
                    .Include(t => t.User)
                    .OrderByDescending(t => t.Flight.DepartureTime);
            }

            return _context.Tickets
                    .Include(t => t.Seat)
                    .Include(t => t.Flight)
                    .ThenInclude(f => f.DepartureAirport)
                    .Include(t => t.Flight)
                    .ThenInclude(f => f.ArrivalAirport)
                    .Include(t => t.User)
                    .Where(t => t.UserId == user.Id)
                    .OrderByDescending(t => t.Flight.DepartureTime);
        }

        public async Task<List<Ticket>> GetTicketsByUserAsync(string userId)
        {
            return await _context.Tickets
        .Include(t => t.Seat)
        .Include(t => t.Flight)
        .ThenInclude(f => f.DepartureAirport)
        .Include(t => t.Flight)
        .ThenInclude(f => f.ArrivalAirport)
        .Where(t => t.UserId == userId)
        .OrderByDescending(t => t.Flight.DepartureTime)
        .ToListAsync();
        }

        public async Task<bool> IsSeatAvailableAsync(int flightId, int seatId)
        {
            return !await _context.Tickets.AnyAsync(t => t.FlightId == flightId && t.SeatId == seatId);
        }

        public async Task<bool> UserHasTicketsAsync(string userId)
        {
            return await _context.Tickets.AnyAsync(u => u.UserId == userId);
        }
    }
}
