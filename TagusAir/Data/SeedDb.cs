using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TagusAir.Data.Entities;
using TagusAir.Helpers;

namespace TagusAir.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {

            await _context.Database.MigrateAsync();

            var user = await _userHelper.GetUserByEmailAsync("danielap@yopmail.com");

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Daniela",
                    LastName = "Pais",
                    Email = "danielap@yopmail.com",
                    UserName = "danielap@yopmail.com",
                    PhoneNumber = "912345678",

                };

                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the admin user in seeder.");
                }
            }

           

            if (!_context.Airplanes.Any())
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
