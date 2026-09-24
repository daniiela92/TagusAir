using System.ComponentModel.DataAnnotations;

namespace TagusAir.Models
{
    public class ChangeUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [MaxLength(20)]
        public string? Passport { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }


    }
}
