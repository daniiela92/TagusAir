using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TagusAir.Data;
using TagusAir.Data.Entities;
using TagusAir.Helpers;

namespace TagusAir.Controllers.API
{
    [Route("api/tickets")]
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
                    t.Id,
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


        [HttpPost("{flightId}/{seatId}")]
        public async Task<IActionResult> BuyTicket(int flightId, int seatId)
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            var isSeatAvailable = await _ticketRepository.IsSeatAvailableAsync(flightId, seatId);

            if (!isSeatAvailable)
            {
                return BadRequest("This seat is no longer available.");
            }

            var boughtTicket = await _ticketRepository.BuyTicketsAsync(flightId, seatId, user.Id);


            var result = new
            {
                boughtTicket.Id,
                boughtTicket.Price,
                boughtTicket.PurchaseDate
            };
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelTicket(int id)
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }


            if (ticket.UserId != user.Id)
            {
                return Forbid();
            }

            await _ticketRepository.DeleteAsync(ticket);

            return NoContent();
        }

        [HttpPut("{id}/{seatId}")]
        public async Task<IActionResult> ChangeSeat(int id, int seatId)
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            if (ticket.UserId != user.Id)
            {
                return Forbid();
            }

            if(ticket.SeatId == seatId)
            {
                return BadRequest("You are already assigned to this seat.");
            }

            var isSeatAvailable = await _ticketRepository.IsSeatAvailableAsync(ticket.FlightId, seatId);
            if (!isSeatAvailable)
            {
                return BadRequest("This seat is no longer available.");
            }
            ticket.SeatId = seatId;
            await _ticketRepository.UpdateAsync(ticket);
            return NoContent();
        }
    }
}
