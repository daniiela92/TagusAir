using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TagusAir.Data.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters length.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters length.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [MaxLength(20, ErrorMessage = "The field {0} can contain {1} characters length.")]
        public string? Passport { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true; 

    }
}
