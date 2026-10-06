using System.ComponentModel.DataAnnotations;

namespace TagusAir.Data.Entities
{
    public class Flight : IEntity
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(10, ErrorMessage = "The field {0} can contain {1} characters lenght.")]
        [Display(Name = "Flight Number")]
        public string FlightNumber { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Departure")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Display(Name = "Arrival")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ArrivalTime { get; set; }

        [Display(Name = "Origin")]
        public int DepartureAirportId { get; set; }

        public Airport? DepartureAirport { get; set; }

        [Display(Name = "Destination")]
        public int ArrivalAirportId { get; set; }

        public Airport? ArrivalAirport { get; set; }

        [Display(Name = "Airplane")]
        public int AirplaneId { get; set; }

        public Airplane? Airplane { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, 10000, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        [Display(Name = "Base Price")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal BasePrice { get; set; }

        [Display(Name = "Business Price")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal BusinessPrice => BasePrice * 2.5m;

        [Display(Name = "Duration")]
        public TimeSpan Duration => ArrivalTime - DepartureTime;

        public override string ToString()
        {
            return $"{FlightNumber} ({DepartureTime:dd/MM/yyyy HH:mm})";
        }
    }
}

