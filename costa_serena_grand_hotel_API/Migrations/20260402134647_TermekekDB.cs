using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace costa_serena_grand_hotel_API.Migrations
{
    /// <inheritdoc />
    public partial class TermekekDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aktiv",
                table: "termek");

            migrationBuilder.AddColumn<int>(
                name: "Darabszam",
                table: "termek",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Darabszam",
                table: "termek");

            migrationBuilder.AddColumn<bool>(
                name: "Aktiv",
                table: "termek",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
