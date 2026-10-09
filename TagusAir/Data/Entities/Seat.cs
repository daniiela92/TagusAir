using System.ComponentModel.DataAnnotations;

namespace TagusAir.Data.Entities
{
    public enum SeatClass
    {
        Economy = 0,

        Business = 1
    }

    public class Seat : IEntity
    {
        public int Id { get; set; }

        public int AirplaneId { get; set; }

        public Airplane? Airplane { get; set; }

        [Required]
        [MaxLength(25)]
        public string? Code { get; set; }

        public SeatClass Class { get; set; }

        public override string ToString()
        {
            return $"{Code} ({Class})";
        }
    }
}
