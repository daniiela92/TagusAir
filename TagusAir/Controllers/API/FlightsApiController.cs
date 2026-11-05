using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TagusAir.Data;
using TagusAir.Data.Entities;

namespace TagusAir.Controllers.API
{
    [Route("api/flights")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FlightsApiController : ControllerBase
    {
        private readonly IFlightRepository _flightRepository;
        private readonly ITicketRepository _ticketRepository;

        public FlightsApiController(
            IFlightRepository  flightRepository,
            ITicketRepository ticketRepository)
        {
            _flightRepository = flightRepository;
            _ticketRepository = ticketRepository;
        }

        [HttpGet]
        public IActionResult GetFlights()
        {
            var flights = _flightRepository.GetAvailableFlights()
                .ToList()
                .Select(f => new
                {
                    f.Id,
                    f.FlightNumber,
                    f.DepartureTime,
                    f.ArrivalTime,
                    Origin = $"{f.DepartureAirport.City} ({f.DepartureAirport.IataCode})",
                    Destination = $"{f.ArrivalAirport.City} ({f.ArrivalAirport.IataCode})",
                    Airplane = $"{f.Airplane.Brand} {f.Airplane.Model}",
                    f.BasePrice
                });

            return Ok(flights);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetFlightById(int id)
        {
            var flight = await _flightRepository.GetFlightWithDetailsAsync(id);
               

            if(flight == null)
            {
                return NotFound();
            }

            var result = new
            {
                flight.Id,
                flight.FlightNumber,
                flight.DepartureTime,
                flight.ArrivalTime,
                Origin = $"{flight.DepartureAirport.City} ({flight.DepartureAirport.IataCode})",
                Destination = $"{flight.ArrivalAirport.City} ({flight.ArrivalAirport.IataCode})",
                Airplane = $"{flight.Airplane.Brand} {flight.Airplane.Model}",
                flight.BasePrice
            };

            return Ok(result);

        }

        [HttpGet("{id}/seats")]
        public async Task<IActionResult> GetAvailableSeats(int id)
        {
            var seats = await _ticketRepository.GetAvailableSeatsAsync(id);

            var availableSeats = seats
                .Select(s => new
                {
                    s.Id,
                    Name= $"{s.Code} - {s.Class}"
                })
                .ToList();


            
            return Ok(availableSeats);
        }

    }
}
