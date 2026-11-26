using System.ComponentModel.DataAnnotations;

namespace TagusAir.Data.Entities
{
    public class Airport :IEntity
    {
        public int Id { get; set; }

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

        public int CountryId { get; set; }

        public Country? Country { get; set; }

        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Range(-180, 180)]
        public double Longitude { get; set; }

        public bool IsActive { get; set; } = true; 



        public override string ToString()
        {
            return $"{City} - {Name} ({IataCode})";
        }
    }
}
