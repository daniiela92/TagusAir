using System.ComponentModel.DataAnnotations;

namespace TagusAir.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        public string? Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [MaxLength(20)]
        public string? Passport { get; set; }
    }
}
