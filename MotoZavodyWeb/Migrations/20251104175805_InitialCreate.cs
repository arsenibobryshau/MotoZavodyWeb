using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoZavodyWeb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hodnoceni",
                columns: table => new
                {
                    IdHodnoceni = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Metoda = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hodnoceni", x => x.IdHodnoceni);
                });

            migrationBuilder.CreateTable(
                name: "Platby",
                columns: table => new
                {
                    IdPlatby = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Castka = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Typ = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platby", x => x.IdPlatby);
                });

            migrationBuilder.CreateTable(
                name: "Posty",
                columns: table => new
                {
                    IdPosta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Psc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mesto = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posty", x => x.IdPosta);
                });

            migrationBuilder.CreateTable(
                name: "Pozice",
                columns: table => new
                {
                    IdPozice = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazev = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pozice", x => x.IdPozice);
                });

            migrationBuilder.CreateTable(
                name: "TypyKolobezek",
                columns: table => new
                {
                    IdTypKolobezky = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazev = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypyKolobezek", x => x.IdTypKolobezky);
                });

            migrationBuilder.CreateTable(
                name: "TypyZavodu",
                columns: table => new
                {
                    IdTypZavodu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazev = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypyZavodu", x => x.IdTypZavodu);
                });

            migrationBuilder.CreateTable(
                name: "Zavodnici",
                columns: table => new
                {
                    IdZavodnik = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Jmeno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prijmeni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vek = table.Column<int>(type: "int", nullable: false),
                    Pohlavi = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    UrovenZkusenosti = table.Column<string>(type: "nvarchar(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zavodnici", x => x.IdZavodnik);
                });

            migrationBuilder.CreateTable(
                name: "Adresy",
                columns: table => new
                {
                    IdAdresa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ulice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cp = table.Column<int>(type: "int", nullable: false),
                    IdPosta = table.Column<int>(type: "int", nullable: false),
                    PostaIdPosta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adresy", x => x.IdAdresa);
                    table.ForeignKey(
                        name: "FK_Adresy_Posty_PostaIdPosta",
                        column: x => x.PostaIdPosta,
                        principalTable: "Posty",
                        principalColumn: "IdPosta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zamestnanci",
                columns: table => new
                {
                    IdZamestnanec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Jmeno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prijmeni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdPozice = table.Column<int>(type: "int", nullable: false),
                    PoziceIdPozice = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zamestnanci", x => x.IdZamestnanec);
                    table.ForeignKey(
                        name: "FK_Zamestnanci_Pozice_PoziceIdPozice",
                        column: x => x.PoziceIdPozice,
                        principalTable: "Pozice",
                        principalColumn: "IdPozice",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kolobezky",
                columns: table => new
                {
                    IdKolobezka = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Znacka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdTypKolobezky = table.Column<int>(type: "int", nullable: false),
                    TypKolobezkyIdTypKolobezky = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kolobezky", x => x.IdKolobezka);
                    table.ForeignKey(
                        name: "FK_Kolobezky_TypyKolobezek_TypKolobezkyIdTypKolobezky",
                        column: x => x.TypKolobezkyIdTypKolobezky,
                        principalTable: "TypyKolobezek",
                        principalColumn: "IdTypKolobezky",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mista",
                columns: table => new
                {
                    IdMisto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazev = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdAdresa = table.Column<int>(type: "int", nullable: false),
                    AdresaIdAdresa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mista", x => x.IdMisto);
                    table.ForeignKey(
                        name: "FK_Mista_Adresy_AdresaIdAdresa",
                        column: x => x.AdresaIdAdresa,
                        principalTable: "Adresy",
                        principalColumn: "IdAdresa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JezdiNa",
                columns: table => new
                {
                    IdZavodnik = table.Column<int>(type: "int", nullable: false),
                    IdKolobezka = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JezdiNa", x => new { x.IdZavodnik, x.IdKolobezka });
                    table.ForeignKey(
                        name: "FK_JezdiNa_Kolobezky_IdKolobezka",
                        column: x => x.IdKolobezka,
                        principalTable: "Kolobezky",
                        principalColumn: "IdKolobezka",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JezdiNa_Zavodnici_IdZavodnik",
                        column: x => x.IdZavodnik,
                        principalTable: "Zavodnici",
                        principalColumn: "IdZavodnik",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zavody",
                columns: table => new
                {
                    IdZavod = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazev = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Startovne = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IdTypZavodu = table.Column<int>(type: "int", nullable: false),
                    IdMisto = table.Column<int>(type: "int", nullable: false),
                    IdHodnoceni = table.Column<int>(type: "int", nullable: false),
                    TypZavoduIdTypZavodu = table.Column<int>(type: "int", nullable: false),
                    MistoIdMisto = table.Column<int>(type: "int", nullable: false),
                    HodnoceniIdHodnoceni = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zavody", x => x.IdZavod);
                    table.ForeignKey(
                        name: "FK_Zavody_Hodnoceni_HodnoceniIdHodnoceni",
                        column: x => x.HodnoceniIdHodnoceni,
                        principalTable: "Hodnoceni",
                        principalColumn: "IdHodnoceni",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zavody_Mista_MistoIdMisto",
                        column: x => x.MistoIdMisto,
                        principalTable: "Mista",
                        principalColumn: "IdMisto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zavody_TypyZavodu_TypZavoduIdTypZavodu",
                        column: x => x.TypZavoduIdTypZavodu,
                        principalTable: "TypyZavodu",
                        principalColumn: "IdTypZavodu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organizace",
                columns: table => new
                {
                    IdZamestnanec = table.Column<int>(type: "int", nullable: false),
                    IdZavod = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizace", x => new { x.IdZamestnanec, x.IdZavod });
                    table.ForeignKey(
                        name: "FK_Organizace_Zamestnanci_IdZamestnanec",
                        column: x => x.IdZamestnanec,
                        principalTable: "Zamestnanci",
                        principalColumn: "IdZamestnanec",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Organizace_Zavody_IdZavod",
                        column: x => x.IdZavod,
                        principalTable: "Zavody",
                        principalColumn: "IdZavod",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ucasti",
                columns: table => new
                {
                    IdZavodnik = table.Column<int>(type: "int", nullable: false),
                    IdZavod = table.Column<int>(type: "int", nullable: false),
                    Poradi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vysledek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdPlatby = table.Column<int>(type: "int", nullable: true),
                    PlatbaIdPlatby = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ucasti", x => new { x.IdZavodnik, x.IdZavod });
                    table.ForeignKey(
                        name: "FK_Ucasti_Platby_PlatbaIdPlatby",
                        column: x => x.PlatbaIdPlatby,
                        principalTable: "Platby",
                        principalColumn: "IdPlatby");
                    table.ForeignKey(
                        name: "FK_Ucasti_Zavodnici_IdZavodnik",
                        column: x => x.IdZavodnik,
                        principalTable: "Zavodnici",
                        principalColumn: "IdZavodnik",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ucasti_Zavody_IdZavod",
                        column: x => x.IdZavod,
                        principalTable: "Zavody",
                        principalColumn: "IdZavod",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adresy_PostaIdPosta",
                table: "Adresy",
                column: "PostaIdPosta");

            migrationBuilder.CreateIndex(
                name: "IX_JezdiNa_IdKolobezka",
                table: "JezdiNa",
                column: "IdKolobezka");

            migrationBuilder.CreateIndex(
                name: "IX_Kolobezky_TypKolobezkyIdTypKolobezky",
                table: "Kolobezky",
                column: "TypKolobezkyIdTypKolobezky");

            migrationBuilder.CreateIndex(
                name: "IX_Mista_AdresaIdAdresa",
                table: "Mista",
                column: "AdresaIdAdresa");

            migrationBuilder.CreateIndex(
                name: "IX_Organizace_IdZavod",
                table: "Organizace",
                column: "IdZavod");

            migrationBuilder.CreateIndex(
                name: "IX_Ucasti_IdZavod",
                table: "Ucasti",
                column: "IdZavod");

            migrationBuilder.CreateIndex(
                name: "IX_Ucasti_PlatbaIdPlatby",
                table: "Ucasti",
                column: "PlatbaIdPlatby");

            migrationBuilder.CreateIndex(
                name: "IX_Zamestnanci_PoziceIdPozice",
                table: "Zamestnanci",
                column: "PoziceIdPozice");

            migrationBuilder.CreateIndex(
                name: "IX_Zavody_HodnoceniIdHodnoceni",
                table: "Zavody",
                column: "HodnoceniIdHodnoceni");

            migrationBuilder.CreateIndex(
                name: "IX_Zavody_MistoIdMisto",
                table: "Zavody",
                column: "MistoIdMisto");

            migrationBuilder.CreateIndex(
                name: "IX_Zavody_TypZavoduIdTypZavodu",
                table: "Zavody",
                column: "TypZavoduIdTypZavodu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JezdiNa");

            migrationBuilder.DropTable(
                name: "Organizace");

            migrationBuilder.DropTable(
                name: "Ucasti");

            migrationBuilder.DropTable(
                name: "Kolobezky");

            migrationBuilder.DropTable(
                name: "Zamestnanci");

            migrationBuilder.DropTable(
                name: "Platby");

            migrationBuilder.DropTable(
                name: "Zavodnici");

            migrationBuilder.DropTable(
                name: "Zavody");

            migrationBuilder.DropTable(
                name: "TypyKolobezek");

            migrationBuilder.DropTable(
                name: "Pozice");

            migrationBuilder.DropTable(
                name: "Hodnoceni");

            migrationBuilder.DropTable(
                name: "Mista");

            migrationBuilder.DropTable(
                name: "TypyZavodu");

            migrationBuilder.DropTable(
                name: "Adresy");

            migrationBuilder.DropTable(
                name: "Posty");
        }
    }
}
