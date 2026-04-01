using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace costa_serena_grand_hotel_API.Migrations
{
    /// <inheritdoc />
    public partial class DbelveteleaFoglaalsbol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Darab",
                table: "szoba_kategoria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Darab",
                table: "szoba_kategoria",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
