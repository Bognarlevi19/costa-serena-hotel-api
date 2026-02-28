using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace costa_serena_grand_hotel_API.Migrations
{
    /// <inheritdoc />
    public partial class VegOsszegFoglalasbolVendegbe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VegOsszeg",
                table: "foglalas");

            migrationBuilder.AddColumn<int>(
                name: "VegOsszeg",
                table: "vendeg",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VegOsszeg",
                table: "vendeg");

            migrationBuilder.AddColumn<int>(
                name: "VegOsszeg",
                table: "foglalas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
