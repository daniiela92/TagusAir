using System.ComponentModel.DataAnnotations;

namespace TagusAir.Data.Entities
{
    public class Country : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(50, ErrorMessage = "The field {0} can have a maximum of {1} characters.")]
        public string Name { get; set; }

        [Display(Name = "Flag Image")]
        public string? FlagImageUrl { get; set; }

        public ICollection<Airport>? Airports { get; set; }

        [Display(Name = "Number of Airports")]
        public int NumberOfAirports => Airports == null ? 0 : Airports.Count;

        public override string ToString()
        {
            return Name;
        }

    }
}
