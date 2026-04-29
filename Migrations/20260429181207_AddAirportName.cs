using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cailean_Razvan_Zboruri.Migrations
{
    /// <inheritdoc />
    public partial class AddAirportName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Airport",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Airport");
        }
    }
}
