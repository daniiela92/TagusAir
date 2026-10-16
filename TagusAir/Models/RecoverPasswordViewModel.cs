using System.ComponentModel.DataAnnotations;

namespace TagusAir.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
