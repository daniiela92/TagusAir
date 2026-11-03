using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TagusAir.Data;

namespace TagusAir.Controllers.API
{
    [Route("api/airplanes")]
    [ApiController]
    public class AirplanesApiController : ControllerBase
    {
        private readonly IAirplaneRepository _airplaneRepository;

        public AirplanesApiController(IAirplaneRepository airplaneRepository)
        {
            _airplaneRepository = airplaneRepository;
        }

        [HttpGet]
        public IActionResult GetAirplanes()
        {
            var airplanes = _airplaneRepository.GetAll()
                .Where(a => a.IsActive)
                .ToList()
                .Select(a => new
                {
                    a.Id,
                    Name = $"{a.Brand} {a.Model}",


                })
                .OrderBy(a => a.Name)
                .ToList();

            return Ok(airplanes);

        }
    }
}
