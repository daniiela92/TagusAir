using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using TagusAir.Data.Entities;
using TagusAir.Helpers;

namespace TagusAir.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IAirplaneRepository _airplaneRepository;

        public SeedDb(DataContext context,
            IUserHelper userHelper,
            IAirplaneRepository airplaneRepository)
        {
            _context = context;
            _userHelper = userHelper;
            _airplaneRepository = airplaneRepository;
        }

        public async Task SeedAsync()
        {

            await _context.Database.MigrateAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Employee");
            await _userHelper.CheckRoleAsync("Customer");

            // Criação de admin 

            var admin = await _userHelper.GetUserByEmailAsync("danielap@yopmail.com");

            if (admin == null)
            {
                admin = new User
                {
                    FirstName = "Daniela",
                    LastName = "Pais",
                    Email = "danielap@yopmail.com",
                    UserName = "danielap@yopmail.com",
                    PhoneNumber = "912345678",

                };

                var adminResult = await _userHelper.AddUserAsync(admin, "123456");

                if (adminResult != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the admin user in seeder.");
                }

                await _userHelper.AddUserToRoleAsync(admin, "Admin");
            }

            var isAdminInRole = await _userHelper.IsUserInRoleAsync(admin, "Admin");

            if (!isAdminInRole) 
            {

                await _userHelper.AddUserToRoleAsync(admin, "Admin");

            }

            // Criação de funcionário

            var employee = await _userHelper.GetUserByEmailAsync("funcionario@yopmail.com");

            if (employee == null)
            {
                employee = new User
                {
                    FirstName = "Funcionario",
                    LastName = "Teste",
                    Email = "funcionario@yopmail.com",
                    UserName = "funcionario@yopmail.com",
                    PhoneNumber = "912345678",

                };

                var employeeResult = await _userHelper.AddUserAsync(employee, "123456");

                if (employeeResult != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the employee user in seeder.");
                }

                await _userHelper.AddUserToRoleAsync(employee, "Employee");
            }

            var isEmployeeInRole = await _userHelper.IsUserInRoleAsync(employee, "Employee");

            if (!isEmployeeInRole) 
            {
                await _userHelper.AddUserToRoleAsync(employee, "Employee");
            }



            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Portugal",
                    FlagImageUrl = "/images/no_image.png",
                    Airports = new List<Airport>
                {
                    new Airport { City = "Lisboa", Name = "Humberto Delgado", IataCode = "LIS" },
                    new Airport { City = "Porto", Name = "Francisco Sá Carneiro", IataCode = "OPO" },
                    new Airport { City = "Faro", Name = "Faro", IataCode = "FAO" }
                }
                });

                _context.Countries.Add(new Country
                {
                    Name = "Spain",
                    FlagImageUrl = "/images/no_image.png",
                    Airports = new List<Airport>
                {
                    new Airport { City = "Madrid", Name = "Adolfo Suárez Barajas", IataCode = "MAD" },
                    new Airport { City = "Barcelona", Name = "El Prat", IataCode = "BCN" }
                }
                });

                _context.Countries.Add(new Country
                {
                    Name = "France",
                    FlagImageUrl = "/images/no_image.png",
                    Airports = new List<Airport>
                {
                    new Airport { City = "Paris", Name = "Charles de Gaulle", IataCode = "CDG" }
                }
                });

                _context.Countries.Add(new Country
                {
                    Name = "United Kingdom",
                    FlagImageUrl = "/images/no_image.png",
                    Airports = new List<Airport>
                {
                     new Airport { City = "London", Name = "Heathrow", IataCode = "LHR" },
                     new Airport { City = "London", Name = "Gatwick", IataCode = "LGW" }
                }
                });

                await _context.SaveChangesAsync();
            }

            if (!_context.Airplanes.Any())
            {
                await AddAirplaneAsync("Airbus", "A320neo", 165, 15);
                await AddAirplaneAsync("Boeing", "737 MAX 8", 162, 16);
                await AddAirplaneAsync("Airbus", "A330-900", 250, 34);
                await AddAirplaneAsync("Embraer", "E195-E2", 132, 0);

            }

            if (!_context.Flights.Any())
            {
                var lisbon = _context.Airports.FirstOrDefault(a => a.IataCode == "LIS");
                var porto = _context.Airports.FirstOrDefault(a => a.IataCode == "OPO");
                var madrid = _context.Airports.FirstOrDefault(a => a.IataCode == "MAD");
                var paris = _context.Airports.FirstOrDefault(a => a.IataCode == "CDG");
                var london = _context.Airports.FirstOrDefault(a => a.IataCode == "LHR");

                var a320 = _context.Airplanes.FirstOrDefault(a => a.Model == "A320neo");
                var b737 = _context.Airplanes.FirstOrDefault(a => a.Model == "737 MAX 8");
                var e195 = _context.Airplanes.FirstOrDefault(a => a.Model == "E195-E2");

                if (lisbon != null && porto != null && madrid != null && paris != null && london != null
                    && a320 != null && b737 != null && e195 != null)
                {
                    _context.Flights.Add(new Flight
                    {
                        FlightNumber = "TA101",
                        DepartureTime = DateTime.Today.AddDays(7).AddHours(8),
                        ArrivalTime = DateTime.Today.AddDays(7).AddHours(9),
                        DepartureAirport = lisbon,
                        ArrivalAirport = porto,
                        Airplane = e195,
                        BasePrice = 49.90m
                    });

                    _context.Flights.Add(new Flight
                    {
                        FlightNumber = "TA201",
                        DepartureTime = DateTime.Today.AddDays(10).AddHours(14).AddMinutes(30),
                        ArrivalTime = DateTime.Today.AddDays(10).AddHours(16).AddMinutes(15),
                        DepartureAirport = lisbon,
                        ArrivalAirport = madrid,
                        Airplane = a320,
                        BasePrice = 89.90m
                    });

                    _context.Flights.Add(new Flight
                    {
                        FlightNumber = "TA301",
                        DepartureTime = DateTime.Today.AddDays(14).AddHours(10),
                        ArrivalTime = DateTime.Today.AddDays(14).AddHours(12).AddMinutes(45),
                        DepartureAirport = lisbon,
                        ArrivalAirport = paris,
                        Airplane = a320,
                        BasePrice = 129.90m
                    });

                    _context.Flights.Add(new Flight
                    {
                        FlightNumber = "TA401",
                        DepartureTime = DateTime.Today.AddDays(21).AddHours(7).AddMinutes(45),
                        ArrivalTime = DateTime.Today.AddDays(21).AddHours(10).AddMinutes(30),
                        DepartureAirport = porto,
                        ArrivalAirport = london,
                        Airplane = b737,
                        BasePrice = 149.90m
                    });

                    await _context.SaveChangesAsync();
                }
            }


          

        }

      
        private async Task AddAirplaneAsync(string brand, string model, int economySeats, int businessSeats)
        {
            await _airplaneRepository.CreateWithSeatsAsync(new Airplane
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
