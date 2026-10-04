using System.ComponentModel.DataAnnotations;
using TagusAir.Data.Entities;

namespace TagusAir.Models
{
    public class AirplaneViewModel : Airplane
    {
        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }
    }
}
