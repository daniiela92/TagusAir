using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TagusAir.Migrations
{
    /// <inheritdoc />
    public partial class TicketUniqueSeatIgnoresCancelled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_FlightId_SeatId",
                table: "Tickets");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_FlightId_SeatId",
                table: "Tickets",
                columns: new[] { "FlightId", "SeatId" },
                unique: true,
                filter: "[IsCanceled] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_FlightId_SeatId",
                table: "Tickets");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_FlightId_SeatId",
                table: "Tickets",
                columns: new[] { "FlightId", "SeatId" },
                unique: true);
        }
    }
}
