using TagusAir.Data.Entities;

namespace TagusAir.Helpers
{
    public interface IPdfHelper
    {
        byte[] GenerateTicketPdf(Ticket ticket, string passengerName);
    }
}
