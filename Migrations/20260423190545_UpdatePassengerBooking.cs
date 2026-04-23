using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cailean_Razvan_Zboruri.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePassengerBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Passenger_PassengerID",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_PassengerID",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "PassengerID",
                table: "Booking");

            migrationBuilder.AlterColumn<string>(
                name: "PassportNumber",
                table: "Passenger",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Passenger",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Passenger",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Passenger",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "BookingID",
                table: "Passenger",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Passenger",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Passenger",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Passenger_BookingID",
                table: "Passenger",
                column: "BookingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Passenger_Booking_BookingID",
                table: "Passenger",
                column: "BookingID",
                principalTable: "Booking",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passenger_Booking_BookingID",
                table: "Passenger");

            migrationBuilder.DropIndex(
                name: "IX_Passenger_BookingID",
                table: "Passenger");

            migrationBuilder.DropColumn(
                name: "BookingID",
                table: "Passenger");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Passenger");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Passenger");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Booking");

            migrationBuilder.AlterColumn<string>(
                name: "PassportNumber",
                table: "Passenger",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Passenger",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Passenger",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Passenger",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassengerID",
                table: "Booking",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_PassengerID",
                table: "Booking",
                column: "PassengerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Passenger_PassengerID",
                table: "Booking",
                column: "PassengerID",
                principalTable: "Passenger",
                principalColumn: "ID");
        }
    }
}
