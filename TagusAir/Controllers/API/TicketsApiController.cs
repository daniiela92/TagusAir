using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TagusAir.Data;
using TagusAir.Data.Entities;
using TagusAir.Helpers;

namespace TagusAir.Controllers.API
{
    [Route("api/mytickets")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TicketsApiController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly ITicketRepository _ticketRepository;

        public TicketsApiController(
            IUserHelper userHelper,
            ITicketRepository ticketRepository)
        {
            _userHelper = userHelper;
            _ticketRepository = ticketRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetBoughtFlights()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);


            if (user == null)
            {
                return NotFound();
            }

            var tickets = await _ticketRepository.GetTicketsByUserAsync(user.Id);

            var futureFlights = tickets.Where(t => t.Flight.DepartureTime > DateTime.Now)
                .Select(t => new
                {
                    t.Flight.FlightNumber,
                    Origin = $"{t.Flight.DepartureAirport.City} ({t.Flight.DepartureAirport.IataCode})",
                    Destination = $"{t.Flight.ArrivalAirport.City} ({t.Flight.ArrivalAirport.IataCode})",
                    t.Flight.DepartureTime,
                    t.Flight.ArrivalTime,
                    Seat = t.Seat.Code,
                    Class = t.Seat.Class.ToString(),
                    t.Price,
                }).ToList();


            return Ok(futureFlights);

        }
    }
}
