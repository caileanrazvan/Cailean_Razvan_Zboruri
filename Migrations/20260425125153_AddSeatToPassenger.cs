using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cailean_Razvan_Zboruri.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatToPassenger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeatNumber",
                table: "Passenger",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatNumber",
                table: "Passenger");
        }
    }
}
