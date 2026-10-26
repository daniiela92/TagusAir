using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TagusAir.Data.Entities;

namespace TagusAir.Models
{
    public class FlightViewModel : Flight
    {

        public IEnumerable<SelectListItem>? Airplanes { get; set; }

        public IEnumerable<SelectListItem>? DepartureAirports { get; set; }

        public IEnumerable<SelectListItem>? ArrivalAirports { get; set; }

        [Display(Name = "Departure Country")]
        public int DepartureCountryId { get; set; }

        [Display(Name = "Arrival Country")]

        public int ArrivalCountryId { get; set;}

        public IEnumerable<SelectListItem>? Countries { get; set; }





    }
}
