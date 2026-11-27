using Microsoft.EntityFrameworkCore;
using TagusAir.Data.Entities;

namespace TagusAir.Data
{
    public class AirplaneRepository : GenericRepository<Airplane>, IAirplaneRepository
    {
        private readonly DataContext _context;

        public AirplaneRepository(DataContext context) : base (context)
        {
            _context = context;
        }

        public async Task<bool> AirplaneHasFlightsAsync(int airplaneId)
        {
            return await _context.Flights.AnyAsync(f => f.AirplaneId == airplaneId);
        }

        public async Task CreateWithSeatsAsync(Airplane airplane)
        {

            await _context.AddAsync(airplane);
            await _context.SaveChangesAsync();

            for(int i = 1; i <= airplane.EconomySeats; i++)
            {
                Seat seat = new Seat
                {
                    AirplaneId = airplane.Id,
                    Code = $"E-{i.ToString("000")}",
                    Class = SeatClass.Economy,
                    

                };

                _context.Seats.Add(seat);
            }

            for(int i = 1; i <= airplane.BusinessSeats; i++)
            {

                Seat seat = new Seat
                {
                    AirplaneId = airplane.Id,
                    Code = $"B-{i.ToString("000")}",
                    Class = SeatClass.Business,


                };

                _context.Seats.Add(seat);

            }

          await _context.SaveChangesAsync();

        }

        public async Task DeleteWithSeatsAsync(Airplane airplane)
        {
            var seats = await _context.Seats.Where(s => s.AirplaneId == airplane.Id)
                .ToListAsync();

            _context.Seats.RemoveRange(seats);

            _context.Airplanes.Remove(airplane);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Ticket>> UpdateWithSeatsAsync(Airplane airplane)
        {
            _context.Airplanes.Update(airplane);
            await _context.SaveChangesAsync();

            await AdjustSeatsAsync(airplane.Id, SeatClass.Economy, airplane.EconomySeats);
            await AdjustSeatsAsync(airplane.Id, SeatClass.Business, airplane.BusinessSeats);

            return await ReseatPassengersAsync(airplane.Id);
        }

        private async Task AdjustSeatsAsync(int airplaneId, SeatClass seatClass, int target)
        {
            var seats = await _context.Seats
                .Where(s => s.AirplaneId == airplaneId && s.Class == seatClass)
                .OrderBy(s => s.Code)
                .ToListAsync();

            var active = seats.Where(s => s.IsActive).ToList();

            if (active.Count > target)
            {
                foreach (var seat in active.Skip(target))
                {
                    seat.IsActive = false;
                }
            }
            else if (active.Count < target)
            {
                var missing = target - active.Count;

                var reusable = seats.Where(s => !s.IsActive).Take(missing).ToList();

                foreach (var seat in reusable)
                {
                    seat.IsActive = true;
                }

                var prefix = seatClass == SeatClass.Economy ? "E" : "B";
                var toCreate = missing - reusable.Count;
                var nextNumber = seats.Count + 1;

                for (int i = 0; i < toCreate; i++)
                {
                    _context.Seats.Add(new Seat
                    {
                        AirplaneId = airplaneId,
                        Code = $"{prefix}-{(nextNumber + i).ToString("000")}",
                        Class = seatClass,
                        IsActive = true
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<List<Ticket>> ReseatPassengersAsync(int airplaneId)
        {
            var tickets = await _context.Tickets
                .Include(t => t.Flight).ThenInclude(f => f.DepartureAirport)
                .Include(t => t.Flight).ThenInclude(f => f.ArrivalAirport)
                .Include(t => t.Seat)
                .Include(t => t.User)
                .Where(t => !t.IsCanceled
                         && t.Flight.AirplaneId == airplaneId
                         && t.Flight.DepartureTime > DateTime.Now
                         && !t.Seat.IsActive)
                .ToListAsync();

            foreach (var ticket in tickets)
            {
                var takenSeats = await _context.Tickets
                    .Where(t => t.FlightId == ticket.FlightId && !t.IsCanceled)
                    .Select(t => t.SeatId)
                    .ToListAsync();

                var newSeat = await _context.Seats
                    .Where(s => s.AirplaneId == airplaneId
                             && s.IsActive
                             && s.Class == ticket.Seat.Class
                             && !takenSeats.Contains(s.Id))
                    .OrderBy(s => s.Code)
                    .FirstOrDefaultAsync();

                if (newSeat == null)
                {
                    ticket.IsCanceled = true;
                }
                else
                {
                    ticket.SeatId = newSeat.Id;
                    ticket.Seat = newSeat;
                }

                await _context.SaveChangesAsync();
            }

            return tickets;
        }
    }
}
