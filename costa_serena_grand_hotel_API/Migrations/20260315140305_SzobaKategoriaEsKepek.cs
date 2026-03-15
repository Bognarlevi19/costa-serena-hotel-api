using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace costa_serena_grand_hotel_API.Migrations
{
    /// <inheritdoc />
    public partial class SzobaKategoriaEsKepek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ferohely",
                table: "szoba",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Leiras",
                table: "szoba",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nev",
                table: "szoba",
                type: "varchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RovidLeiras",
                table: "szoba",
                type: "varchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SzobaKategoriaId",
                table: "szoba",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "szoba_kategoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Nev = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    Leiras = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_szoba_kategoria", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "szoba_kep",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    KepUrl = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    FoKep = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sorrend = table.Column<int>(type: "int", nullable: false),
                    SzobaId = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_szoba_SzobaKategoriaId",
                table: "szoba",
                column: "SzobaKategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_szoba_kep_SzobaId",
                table: "szoba_kep",
                column: "SzobaId");

            migrationBuilder.AddForeignKey(
                name: "FK_szoba_szoba_kategoria_SzobaKategoriaId",
                table: "szoba",
                column: "SzobaKategoriaId",
                principalTable: "szoba_kategoria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_szoba_szoba_kategoria_SzobaKategoriaId",
                table: "szoba");

            migrationBuilder.DropTable(
                name: "szoba_kategoria");

            migrationBuilder.DropTable(
                name: "szoba_kep");

            migrationBuilder.DropIndex(
                name: "IX_szoba_SzobaKategoriaId",
                table: "szoba");

            migrationBuilder.DropColumn(
                name: "Ferohely",
                table: "szoba");

            migrationBuilder.DropColumn(
                name: "Leiras",
                table: "szoba");

            migrationBuilder.DropColumn(
                name: "Nev",
                table: "szoba");

            migrationBuilder.DropColumn(
                name: "RovidLeiras",
                table: "szoba");

            migrationBuilder.DropColumn(
                name: "SzobaKategoriaId",
                table: "szoba");
        }
    }
}
