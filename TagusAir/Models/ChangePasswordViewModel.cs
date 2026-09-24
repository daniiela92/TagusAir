using System.ComponentModel.DataAnnotations;

namespace TagusAir.Models
{
    public class ChangePasswordViewModel
    {

        [Required]
        [Display(Name = "Current Password")]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(6)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation do not match.")]
        public string Confirm { get; set; }
    }
}
