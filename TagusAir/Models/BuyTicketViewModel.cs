using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TagusAir.Data.Entities;

namespace TagusAir.Models
{
    public class BuyTicketViewModel
    {
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters length.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters length.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [MaxLength(20, ErrorMessage = "The field {0} can contain {1} characters length.")]
        [Display(Name = "Passport")]
        public string? Passport { get; set; }

        [MaxLength(20, ErrorMessage = "The field {0} can contain {1} characters length.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public int FlightId { get; set; }

        public Flight? Flight {get;set;}

        [Required(ErrorMessage = "You must select a seat")]
        [Display(Name = "Seat")]
        public int SeatId { get; set;}

        public IEnumerable<SelectListItem>? Seats { get; set; }

    }
}
