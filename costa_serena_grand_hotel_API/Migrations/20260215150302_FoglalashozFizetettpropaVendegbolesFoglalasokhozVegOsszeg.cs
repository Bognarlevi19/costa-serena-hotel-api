using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace costa_serena_grand_hotel_API.Migrations
{
    /// <inheritdoc />
    public partial class FoglalashozFizetettpropaVendegbolesFoglalasokhozVegOsszeg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fizetett",
                table: "vendeg");

            migrationBuilder.AddColumn<bool>(
                name: "Fizetett",
                table: "foglalas",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "VegOsszeg",
                table: "foglalas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fizetett",
                table: "foglalas");

            migrationBuilder.DropColumn(
                name: "VegOsszeg",
                table: "foglalas");

            migrationBuilder.AddColumn<bool>(
                name: "Fizetett",
                table: "vendeg",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
