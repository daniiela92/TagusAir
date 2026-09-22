using System.ComponentModel.DataAnnotations;

namespace TagusAir.Data.Entities
{
    public class Airplane : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters lenght.")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters lenght.")]
        public string Model { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The field {0} cannot be negative.")]
        [Display(Name = "Economy Seats")]
        public int EconomySeats { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The field {0} cannot be negative.")]
        [Display(Name = "Business Seats")]
        public int BusinessSeats { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        public override string ToString()
        {
            return $"{Brand} {Model}";
        }

    }

}

