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
            var flights = _flightRepository.GetAllWithDetails()
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

        [HttpPost]
        public async Task<IActionResult> Post(Flight newFlight)
        {
            await _flightRepository.CreateAsync(newFlight);

            return Ok(newFlight);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Flight flight)
        {
            if(id != flight.Id)
            {
                return BadRequest();
            }

            var flightExists = await _flightRepository.ExistAsync(id);

            if(!flightExists)
            {
                return NotFound();
            }

            await _flightRepository.UpdateAsync(flight);
            return NoContent(); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {


            var flight = await _flightRepository.GetByIdAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            if(await _ticketRepository.FlightHasTicketsAsync(id))
            {
                return BadRequest("This flight has sold tickets and cannot be deleted.");
            }

            await _flightRepository.DeleteAsync(flight);
            return NoContent();
        }

    }
}
