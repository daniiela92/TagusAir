using System.ComponentModel.DataAnnotations;

namespace TagusAir.Data.Entities
{
    public class Ticket : IEntity
    {
        public int Id { get; set; }

        public int FlightId { get; set; }

        public Flight? Flight { get; set; }

        public int SeatId { get; set; }

        public Seat? Seat { get; set; }

        public string UserId { get; set; }

        public User? User { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        [Display(Name = "Purchase Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime PurchaseDate { get; set; }






    }
}
