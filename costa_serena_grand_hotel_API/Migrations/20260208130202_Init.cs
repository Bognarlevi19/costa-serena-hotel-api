using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace costa_serena_grand_hotel_API.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "szoba",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Szam = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false),
                    Emelet = table.Column<int>(type: "int", nullable: false),
                    Alapterulet = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_szoba", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vendeg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SzemelyiIgazolvanySzam = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Nev = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    IranyitoSzam = table.Column<int>(type: "int", nullable: false),
                    Varos = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    Utca = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Hazszam = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    Fizetett = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vendeg", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "foglalas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SzobaId = table.Column<int>(type: "int", nullable: false),
                    VendegId = table.Column<int>(type: "int", nullable: false),
                    Mettol = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Meddig = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_foglalas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_foglalas_szoba_SzobaId",
                        column: x => x.SzobaId,
                        principalTable: "szoba",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_foglalas_vendeg_VendegId",
                        column: x => x.VendegId,
                        principalTable: "vendeg",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_foglalas_SzobaId",
                table: "foglalas",
                column: "SzobaId");

            migrationBuilder.CreateIndex(
                name: "IX_foglalas_VendegId",
                table: "foglalas",
                column: "VendegId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "foglalas");

            migrationBuilder.DropTable(
                name: "szoba");

            migrationBuilder.DropTable(
                name: "vendeg");
        }
    }
}
