using System.ComponentModel.DataAnnotations;
using TagusAir.Data.Entities;

namespace TagusAir.Models
{
    public class CountryViewModel : Country
    {
        [Display(Name = "Flag")]
        public IFormFile? FlagFile { get; set; }

    }
}
