using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cailean_Razvan_Zboruri.Migrations
{
    /// <inheritdoc />
    public partial class PassengerAmenities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AmenityPassenger",
                columns: table => new
                {
                    AmenitiesID = table.Column<int>(type: "int", nullable: false),
                    PassengersID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmenityPassenger", x => new { x.AmenitiesID, x.PassengersID });
                    table.ForeignKey(
                        name: "FK_AmenityPassenger_Amenity_AmenitiesID",
                        column: x => x.AmenitiesID,
                        principalTable: "Amenity",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AmenityPassenger_Passenger_PassengersID",
                        column: x => x.PassengersID,
                        principalTable: "Passenger",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AmenityPassenger_PassengersID",
                table: "AmenityPassenger",
                column: "PassengersID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AmenityPassenger");
        }
    }
}
