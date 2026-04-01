using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace costa_serena_grand_hotel_API.Migrations
{
    /// <inheritdoc />
    public partial class Adminlistak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Elkuldve",
                table: "rendeles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Elkuldve",
                table: "rendeles");
        }
    }
}
