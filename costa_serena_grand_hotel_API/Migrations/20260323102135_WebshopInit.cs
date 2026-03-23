using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace costa_serena_grand_hotel_API.Migrations
{
    /// <inheritdoc />
    public partial class WebshopInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rendeles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    VendegId = table.Column<int>(type: "int", nullable: false),
                    Nev = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    SzemelyiIgazolvanySzam = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    IranyitoSzam = table.Column<int>(type: "int", nullable: false),
                    Varos = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    Utca = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Hazszam = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    Letrehozva = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Vegosszeg = table.Column<int>(type: "int", nullable: false),
                    Fizetett = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rendeles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rendeles_vendeg_VendegId",
                        column: x => x.VendegId,
                        principalTable: "vendeg",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "termek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Nev = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    Leiras = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Ar = table.Column<int>(type: "int", nullable: false),
                    KepUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    Kategoria = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true),
                    Aktiv = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_termek", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rendeles_tetel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RendelesId = table.Column<int>(type: "int", nullable: false),
                    TermekId = table.Column<int>(type: "int", nullable: false),
                    Mennyiseg = table.Column<int>(type: "int", nullable: false),
                    Egysegar = table.Column<int>(type: "int", nullable: false),
                    Osszeg = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rendeles_tetel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rendeles_tetel_rendeles_RendelesId",
                        column: x => x.RendelesId,
                        principalTable: "rendeles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rendeles_tetel_termek_TermekId",
                        column: x => x.TermekId,
                        principalTable: "termek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_rendeles_VendegId",
                table: "rendeles",
                column: "VendegId");

            migrationBuilder.CreateIndex(
                name: "IX_rendeles_tetel_RendelesId",
                table: "rendeles_tetel",
                column: "RendelesId");

            migrationBuilder.CreateIndex(
                name: "IX_rendeles_tetel_TermekId",
                table: "rendeles_tetel",
                column: "TermekId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rendeles_tetel");

            migrationBuilder.DropTable(
                name: "rendeles");

            migrationBuilder.DropTable(
                name: "termek");
        }
    }
}
