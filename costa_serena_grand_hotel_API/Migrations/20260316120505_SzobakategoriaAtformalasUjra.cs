using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace costa_serena_grand_hotel_API.Migrations
{
    /// <inheritdoc />
    public partial class SzobakategoriaAtformalasUjra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KepEleresiUt",
                table: "szoba_kategoria");

            migrationBuilder.AddColumn<string>(
                name: "KepekJson",
                table: "szoba_kategoria",
                type: "json",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KepekJson",
                table: "szoba_kategoria");

            migrationBuilder.AddColumn<string>(
                name: "KepEleresiUt",
                table: "szoba_kategoria",
                type: "varchar(250)",
                maxLength: 250,
                nullable: true);
        }
    }
}
