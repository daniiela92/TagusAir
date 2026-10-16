using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TagusAir.Data.Entities;

namespace TagusAir.Models
{
    public class BuyTicketViewModel
    {

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public int FlightId { get; set; }

        public Flight? Flight {get;set;}

        [Required(ErrorMessage = "You must select a seat")]
        [Display(Name = "Seat")]
        public int SeatId { get; set;}

        public IEnumerable<SelectListItem>? Seats { get; set; }

    }
}
