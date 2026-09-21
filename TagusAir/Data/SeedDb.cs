using Microsoft.EntityFrameworkCore;
using TagusAir.Data.Entities;

namespace TagusAir.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {

            await _context.Database.MigrateAsync();

            if(!_context.Airplanes.Any())
            {
                AddAirplane("Airbus", "A320neo", 165, 15);
                AddAirplane("Boeing", "737 MAX 8", 162, 16);
                AddAirplane("Airbus", "A330-900", 250, 34);
                AddAirplane("Embraer", "E195-E2", 132, 0);

                await _context.SaveChangesAsync();
            }

        }


        private void AddAirplane(string brand, string model, int economySeats, int businessSeats)
        {
            _context.Airplanes.Add(new Airplane
            {
                Brand = brand,
                Model = model,
                EconomySeats = economySeats,
                BusinessSeats = businessSeats,
                IsActive = true,
                ImageUrl = "/images/no_image.png"
            });
        } 
    }
}
