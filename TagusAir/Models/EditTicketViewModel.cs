using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TagusAir.Data.Entities;

namespace TagusAir.Models
{
    public class EditTicketViewModel
    {
        public int Id { get; set; }

        public Flight? Flight { get; set; }

        [Display(Name = "Current seat")]
        public string? CurrentSeat { get; set; }

        [Display(Name = "New seat")]
        public int SeatId { get; set; }

        public IEnumerable<SelectListItem>? Seats { get; set; }
    }
}
