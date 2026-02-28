using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace costa_serena_grand_hotel_API.Migrations
{
    /// <inheritdoc />
    public partial class VegOsszegtorleseaDBbol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VegOsszeg",
                table: "vendeg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VegOsszeg",
                table: "vendeg",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
