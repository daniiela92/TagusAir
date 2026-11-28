using Syncfusion.Pdf;
using Syncfusion.Pdf.Barcode;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using TagusAir.Data.Entities;

namespace TagusAir.Helpers
{
    public class PdfHelper : IPdfHelper
    {
        public byte[] GenerateTicketPdf(Ticket ticket, string passengerName)
        {
            using (var document = new PdfDocument())
            {
                var page = document.Pages.Add();
                var graphics = page.Graphics;

                var brand = new PdfSolidBrush(new PdfColor(48, 17, 188));
                var dark = new PdfSolidBrush(new PdfColor(26, 26, 26));
                var grey = new PdfSolidBrush(new PdfColor(120, 120, 120));

                var brandFont = new PdfStandardFont(PdfFontFamily.Helvetica, 22, PdfFontStyle.Bold);
                var routeFont = new PdfStandardFont(PdfFontFamily.Helvetica, 30, PdfFontStyle.Bold);
                var labelFont = new PdfStandardFont(PdfFontFamily.Helvetica, 9);
                var valueFont = new PdfStandardFont(PdfFontFamily.Helvetica, 13, PdfFontStyle.Bold);

                graphics.DrawString("TagusAir", brandFont, brand, new PointF(0, 0));
                graphics.DrawString("Boarding pass", labelFont, grey, new PointF(0, 30));

                graphics.DrawLine(new PdfPen(grey, 0.5f), new PointF(0, 55), new PointF(515, 55));

                graphics.DrawString(ticket.Flight.DepartureAirport.IataCode, routeFont, dark, new PointF(0, 80));
                graphics.DrawString("to", labelFont, grey, new PointF(115, 95));
                graphics.DrawString(ticket.Flight.ArrivalAirport.IataCode, routeFont, dark, new PointF(145, 80));

                graphics.DrawString(ticket.Flight.DepartureAirport.City, labelFont, grey, new PointF(0, 118));
                graphics.DrawString(ticket.Flight.ArrivalAirport.City, labelFont, grey, new PointF(145, 118));

                DrawField(graphics, labelFont, valueFont, grey, dark, 0, 160, "Passenger", passengerName);

                DrawField(graphics, labelFont, valueFont, grey, dark, 0, 210, "Flight", ticket.Flight.FlightNumber);
                DrawField(graphics, labelFont, valueFont, grey, dark, 120, 210, "Seat", ticket.Seat.Code);
                DrawField(graphics, labelFont, valueFont, grey, dark, 220, 210, "Class", ticket.Seat.Class.ToString());

                DrawField(graphics, labelFont, valueFont, grey, dark, 0, 260, "Departure",
                    ticket.Flight.DepartureTime.ToString("dd/MM/yyyy HH:mm"));
                DrawField(graphics, labelFont, valueFont, grey, dark, 150, 260, "Arrival",
                    ticket.Flight.ArrivalTime.ToString("dd/MM/yyyy HH:mm"));
                DrawField(graphics, labelFont, valueFont, grey, dark, 300, 260, "Price",
                    ticket.Price.ToString("0.00") + " EUR");

                var reference = $"TAGUSAIR-{ticket.Id}";

                var qrBarcode = new PdfQRBarcode();
                qrBarcode.Text = reference;
                qrBarcode.Size = new SizeF(120, 120);
                qrBarcode.Draw(page, new PointF(380, 70));

                graphics.DrawString(reference, labelFont, grey, new PointF(380, 196));

                graphics.DrawLine(new PdfPen(grey, 0.5f), new PointF(0, 320), new PointF(515, 320));
                graphics.DrawString("Please be at the gate 30 minutes before departure.",
                    labelFont, grey, new PointF(0, 332));

                using (var stream = new MemoryStream())
                {
                    document.Save(stream);
                    return stream.ToArray();
                }
            }
        }

        private void DrawField(PdfGraphics graphics, PdfFont labelFont, PdfFont valueFont,
            PdfBrush labelBrush, PdfBrush valueBrush, float x, float y, string label, string value)
        {
            graphics.DrawString(label, labelFont, labelBrush, new PointF(x, y));
            graphics.DrawString(value, valueFont, valueBrush, new PointF(x, y + 14));
        }
    }
}

