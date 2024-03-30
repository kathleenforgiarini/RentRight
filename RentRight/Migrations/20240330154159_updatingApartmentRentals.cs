using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentRight.Migrations
{
    /// <inheritdoc />
    public partial class updatingApartmentRentals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rental_Apartment_ApartmentId",
                table: "Rental");

            migrationBuilder.AlterColumn<int>(
                name: "ApartmentId",
                table: "Rental",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ApartmentNumber",
                table: "Rental",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PropertyId",
                table: "Rental",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rental_PropertyId",
                table: "Rental",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rental_Apartment_ApartmentId",
                table: "Rental",
                column: "ApartmentId",
                principalTable: "Apartment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rental_Property_PropertyId",
                table: "Rental",
                column: "PropertyId",
                principalTable: "Property",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rental_Apartment_ApartmentId",
                table: "Rental");

            migrationBuilder.DropForeignKey(
                name: "FK_Rental_Property_PropertyId",
                table: "Rental");

            migrationBuilder.DropIndex(
                name: "IX_Rental_PropertyId",
                table: "Rental");

            migrationBuilder.DropColumn(
                name: "ApartmentNumber",
                table: "Rental");

            migrationBuilder.DropColumn(
                name: "PropertyId",
                table: "Rental");

            migrationBuilder.AlterColumn<int>(
                name: "ApartmentId",
                table: "Rental",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rental_Apartment_ApartmentId",
                table: "Rental",
                column: "ApartmentId",
                principalTable: "Apartment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
