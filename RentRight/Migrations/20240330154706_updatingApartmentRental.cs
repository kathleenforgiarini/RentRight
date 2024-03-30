using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentRight.Migrations
{
    /// <inheritdoc />
    public partial class updatingApartmentRental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rental_Apartment_ApartmentId",
                table: "Rental");

            migrationBuilder.DropIndex(
                name: "IX_Rental_ApartmentId",
                table: "Rental");

            migrationBuilder.DropColumn(
                name: "ApartmentId",
                table: "Rental");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApartmentId",
                table: "Rental",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rental_ApartmentId",
                table: "Rental",
                column: "ApartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rental_Apartment_ApartmentId",
                table: "Rental",
                column: "ApartmentId",
                principalTable: "Apartment",
                principalColumn: "Id");
        }
    }
}
