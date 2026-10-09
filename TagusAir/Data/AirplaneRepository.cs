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
    }
}
