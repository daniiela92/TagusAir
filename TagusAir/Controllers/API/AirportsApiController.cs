using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TagusAir.Data;

namespace TagusAir.Controllers.API
{
    [Route("api/airports")]
    [ApiController]
    public class AirportsApiController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;

        public AirportsApiController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        [HttpGet]
        public IActionResult GetAllAirports()
        {
            var airports = _countryRepository.GetAllAirports()
                .ToList()
                .Select(a => new
                {
                    a.Id,
                    Name = $"{a.City} ({a.IataCode})",
                })
                .OrderBy(a => a.Name)
                .ToList();

            return Ok(airports);


        }

    }
}
