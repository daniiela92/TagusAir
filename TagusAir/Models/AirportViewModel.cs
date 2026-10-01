using System.ComponentModel.DataAnnotations;

namespace TagusAir.Models
{
    public class AirportViewModel
    {
        public int CountryId { get; set; }

        public int AirportId { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(80, ErrorMessage = "The field {0} can contain {1} characters lenght.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters lenght.")]
        public string City { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(3, ErrorMessage = "The field {0} can contain {1} characters lenght.")]
        [Display(Name = "IATA Code")]
        public string IataCode { get; set; }

    }
}
