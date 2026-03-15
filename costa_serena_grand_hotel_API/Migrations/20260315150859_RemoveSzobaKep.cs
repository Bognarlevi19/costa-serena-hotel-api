using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace costa_serena_grand_hotel_API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSzobaKep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "szoba_kep");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "szoba_kep",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SzobaId = table.Column<int>(type: "int", nullable: false),
                    FoKep = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    KepUrl = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    Sorrend = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_szoba_kep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_szoba_kep_szoba_SzobaId",
                        column: x => x.SzobaId,
                        principalTable: "szoba",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_szoba_kep_SzobaId",
                table: "szoba_kep",
                column: "SzobaId");
        }
    }
}
