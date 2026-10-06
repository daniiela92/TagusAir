using Microsoft.AspNetCore.Mvc.Rendering;
using TagusAir.Data.Entities;

namespace TagusAir.Models
{
    public class FlightViewModel : Flight
    {

        public IEnumerable<SelectListItem>? Airplanes { get; set; }

        public IEnumerable<SelectListItem>? DepartureAirports { get; set; }

        public IEnumerable<SelectListItem>? ArrivalAirports { get; set; }



    }
}
