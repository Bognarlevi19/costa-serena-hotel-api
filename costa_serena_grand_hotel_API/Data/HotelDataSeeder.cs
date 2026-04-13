using costa_serena_grand_hotel_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace costa_serena_grand_hotel_API.Data
{
    public static class HotelDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HotelDbContext>();

            await context.Database.MigrateAsync();

            await SeedSzobaKategoriakAsync(context);
            await SeedSzobakAsync(context);
            await SeedTermekekAsync(context);
        }

        private static async Task SeedSzobaKategoriakAsync(HotelDbContext context)
        {
            if (await context.SzobaKategoriak.AnyAsync())
                return;

            var kategoriak = new List<SzobaKategoria>
            {
                new SzobaKategoria
                {
                    Nev = "Basic",
                    Leiras = "Kedvező árú, egyszerűen berendezett szobák rövidebb és hosszabb tartózkodásra.",
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/Basicszoba.png"
                    })
                },
                new SzobaKategoria
                {
                    Nev = "Deluxe",
                    Leiras = "Tágasabb, elegánsabb szobák jobb felszereltséggel és kényelmesebb kialakítással.",
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/DeluxeSeaBreeze.png"
                    })
                },
                new SzobaKategoria
                {
                    Nev = "Luxus",
                    Leiras = "Prémium kialakítású szobák exkluzív hangulattal és magasabb szintű kényelemmel.",
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/LuxusOcean.png"
                    })
                },
                new SzobaKategoria
                {
                    Nev = "Lakosztály",
                    Leiras = "A legmagasabb kategória, különleges térérzettel és kiemelt felszereltséggel.",
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/panorama.png"
                    })
                }
            };

            context.SzobaKategoriak.AddRange(kategoriak);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSzobakAsync(HotelDbContext context)
        {
            if (await context.Szobak.AnyAsync())
                return;

            var kategoriak = await context.SzobaKategoriak
                .ToDictionaryAsync(k => k.Nev, k => k.Id);

            var szobak = new List<Szoba>
            {
                // ---- BASIC KATEGÓRIA ----
                new Szoba
                {
                    Szam = "101",
                    Emelet = 1,
                    Alapterulet = 22.5,
                    Ar = 32990,
                    Nev = "Basic Garden",
                    RovidLeiras = "Egyszerű és kényelmes basic szoba kertre néző ablakkal.",
                    Leiras = "Egyszerű és kényelmes basic szoba kertre néző ablakkal.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Basic"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/Basicszoba.png",
                        "/kepek/Szobak/Basic101furdo.png",
                        "/kepek/Szobak/basicgardenterasz.png",
                        "/kepek/Szobak/basicgardenplusz.png",
                    })
                },
                new Szoba
                {
                    Szam = "102",
                    Emelet = 1,
                    Alapterulet = 24,
                    Ar = 33990,
                    Nev = "Basic Comfort",
                    RovidLeiras = "Világos basic szoba letisztult berendezéssel.",
                    Leiras = "Világos basic szoba letisztult berendezéssel.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Basic"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/BasicComfort.png",
                        "/kepek/Szobak/Basic101furdo.png",
                        "/kepek/Szobak/Basic comfort terasz.png",
                        "/kepek/Szobak/basic comfort plusz.jpg"
                    })
                },
                new Szoba
                {
                    Szam = "103",
                    Emelet = 1,
                    Alapterulet = 25,
                    Ar = 34990,
                    Nev = "Basic Twin",
                    RovidLeiras = "Két külön ággyal rendelkező basic szoba.",
                    Leiras = "Két külön ággyal rendelkező basic szoba.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Basic"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/Basic102 (2).png",
                        "/kepek/Szobak/Basic102furdo.png",
                        "/kepek/Szobak/basic twin terasz.jpg",
                        "/kepek/Szobak/basic twin plusz.jpg"
                    })
                },

                // ---- DELUXE KATEGÓRIA ----
                new Szoba
                {
                    Szam = "201",
                    Emelet = 2,
                    Alapterulet = 31.5,
                    Ar = 45990,
                    Nev = "Deluxe Sea Breeze",
                    RovidLeiras = "Elegáns deluxe szoba részleges tengerre néző kilátással.",
                    Leiras = "Elegáns deluxe szoba részleges tengerre néző kilátással.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Deluxe"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/DeluxeSeaBreeze.png",
                        "/kepek/Szobak/Deluxe201furdo.png",
                        "/kepek/Szobak/deluxe sea breeze terasz.jpg",
                        "/kepek/Szobak/deluxe breeze plusz.jpg"
                    })
                },
                new Szoba
                {
                    Szam = "202",
                    Emelet = 2,
                    Alapterulet = 33,
                    Ar = 47990,
                    Nev = "Deluxe Balcony",
                    RovidLeiras = "Deluxe szoba saját erkéllyel és tágas belső térrel.",
                    Leiras = "Deluxe szoba saját erkéllyel és tágas belső térrel.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Deluxe"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/Deluxe202.png",
                        "/kepek/Szobak/Deluxe202furdo.png",
                        "/kepek/Szobak/Balcony-terasz.png",
                        "/kepek/Szobak/Balcony-Komod.png"
                    })
                },
                new Szoba
                {
                    Szam = "203",
                    Emelet = 2,
                    Alapterulet = 35,
                    Ar = 49990,
                    Nev = "Deluxe Family",
                    RovidLeiras = "Nagyobb deluxe szoba kisebb családok számára.",
                    Leiras = "Nagyobb deluxe szoba kisebb családok számára.",
                    Ferohely = 3,
                    SzobaKategoriaId = kategoriak["Deluxe"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        //"/kepek/Szobak/Luxus301.png",
                        //"/kepek/Szobak/Luxus301furdo.png"
                        "/kepek/Szobak/Deluxe203.png",
                        "/kepek/Szobak/Deluxe203furdo.png",
                        "/kepek/Szobak/Family-terasz.png",
                        "/kepek/Szobak/Family-szekreny.png"
                    })
                },

                // ---- LUXUS KATEGÓRIA ----
                new Szoba
                {
                    Szam = "301",
                    Emelet = 3,
                    Alapterulet = 42,
                    Ar = 69990,
                    Nev = "Luxus Ocean View",
                    RovidLeiras = "Prémium luxus szoba teljes óceáni panorámával.",
                    Leiras = "Prémium luxus szoba teljes óceáni panorámával.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Luxus"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/LuxusOcean.png",
                        "/kepek/Szobak/LuxusOceankad.png",
                        "/kepek/Szobak/Oceanterasz.png",
                        "/kepek/Szobak/Ocean-komod.png",
                    })
                },
                new Szoba
                {
                    Szam = "302",
                    Emelet = 3,
                    Alapterulet = 45,
                    Ar = 74990,
                    Nev = "Luxus Serenity",
                    RovidLeiras = "Luxus szoba nyugodt, exkluzív hangulattal.",
                    Leiras = "Luxus szoba nyugodt, exkluzív hangulattal.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Luxus"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                       // "/kepek/Szobak/Lakosztaly501furdo.png",
                       "/kepek/Szobak/LuxusSerenity.png",
                        "/kepek/Szobak/Luxus302furdo.png",
                        "/kepek/Szobak/Serenity-terasz.png",
                        "/kepek/Szobak/Serenity-komód.png",

                    })
                },
                 new Szoba
                {
                    Szam = "303",
                    Emelet = 3,
                    Alapterulet = 48,
                    Ar = 79990,
                    Nev = "Luxus Signature",
                    RovidLeiras = "Exkluzív luxus szoba különleges enteriőrrel.",
                    Leiras = "Exkluzív luxus szoba különleges enteriőrrel.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Luxus"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        //"/kepek/Szobak/Lakosztaly502.png",
                        //"/kepek/Szobak/Lakosztaly502furdo.png"
                        "/kepek/Szobak/Luxus303.png",
                        "/kepek/Szobak/Luxus303furdo.png",
                        "/kepek/Szobak/Signature-terasz.png",
                        "/kepek/Szobak/SignatureKomod.png"
                    })
                },

                // ---- LAKOSZTÁLY KATEGÓRIA ----
                new Szoba
                {
                    Szam = "401",
                    Emelet = 4,
                    Alapterulet = 65,
                    Ar = 109990,
                    Nev = "Lakosztály Panorama",
                    RovidLeiras = "Tágas lakosztály külön nappali résszel és panorámával.",
                    Leiras = "Tágas lakosztály külön nappali résszel és panorámával.",
                    Ferohely = 4,
                    SzobaKategoriaId = kategoriak["Lakosztály"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/panorama.png",
                        "/kepek/Szobak/panorama-furdo.png",
                        "/kepek/Szobak/panorama-terasz.png",
                        "/kepek/Szobak/panorama-plafon.png"
                    })
                },
                new Szoba
                {
                    Szam = "402",
                    Emelet = 4,
                    Alapterulet = 72,
                    Ar = 119990,
                    Nev = "Lakosztály Royal",
                    RovidLeiras = "Prémium lakosztály elegáns és reprezentatív kialakítással.",
                    Leiras = "Prémium lakosztály elegáns és reprezentatív kialakítással.",
                    Ferohely = 4,
                    SzobaKategoriaId = kategoriak["Lakosztály"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/Royal.png",
                        "/kepek/Szobak/Royal-fürdo.png",
                        "/kepek/Szobak/Royal-terasz.png",
                        "/kepek/Szobak/Royal-plafon.png"
                    })
                },
                new Szoba
                {
                    Szam = "403",
                    Emelet = 4,
                    Alapterulet = 80,
                    Ar = 129990,
                    Nev = "Lakosztály Presidential",
                    RovidLeiras = "A szálloda legkiemeltebb lakosztálya.",
                    Leiras = "A szálloda legkiemeltebb lakosztálya.",
                    Ferohely = 4,
                    SzobaKategoriaId = kategoriak["Lakosztály"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/Presidental.png",
                        "/kepek/Szobak/Presidental-furdo.png",
                        "/kepek/Szobak/Presidantel-terasz.png",
                        "/kepek/Szobak/Presidental - tancosgare.png"
                    })
                }
            };

            context.Szobak.AddRange(szobak);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTermekekAsync(HotelDbContext context)
        {
            if (await context.Termekek.AnyAsync())
                return;

            var termekek = new List<Termek>
            {
                new Termek
                {
                    Nev = "Costa Serena strandtörölköző",
                    Leiras = "Puha, nagy méretű saját márkás strandtörölköző.",
                    Ar = 7990,
                    KepUrl = "/kepek/Termekek/strandtorolkozo.png",
                    Kategoria = "Textil",
                    Darabszam = 20
                },
                new Termek
                {
                    Nev = "Costa Serena Dísztakaró",
                    Leiras = "Elegáns, bézs színű, halszálkamintás és rojtozott végű puha takaró, amely stílusos, meleg kiegészítője a belső tereknek.",
                    Ar = 12990,
                    KepUrl = "/kepek/Termekek/disztakaro.png",
                    Kategoria = "Textil",
                    Darabszam = 20
                },
                new Termek
                {
                    Nev = "Costa Serena Illatgyertya",
                    Leiras = " Ez a letisztult, fekete üveges, arany címkével ellátott illatgyertya a luxus tengerparti atmoszférát csempészi be a mindennapokba.",
                    Ar = 3990,
                    KepUrl = "/kepek/Termekek/Illatgyertya.png",
                    Kategoria = "Ajándék",
                    Darabszam = 20
                },
                new Termek
                {
                    Nev = "Costa Serena Kollekció",
                    Leiras = "Átfogó prémium válogatás a márka exkluzív fekete-arany arculatú termékeiből, amely a gyertyát, a takarót, valamint a kényeztető fürdőkozmetikumokat és fürdőbombákat is egyaránt tartalmazza.",
                    Ar = 19900,
                    KepUrl = "/kepek/Termekek/shopkollekcio.png",
                    Kategoria = "Ajándék",
                    Darabszam = 20
                },
                new Termek
                {
                    Nev = "Costa Serena Karkötő",
                    Leiras = "Ez a kifinomult, letisztult dizájnú és finom arany részletekkel díszített karkötő tökéletes, exkluzív kiegészítőként hordozza magában a felhőtlen, luxus tengerparti életérzést.",
                    Ar = 6990,
                    KepUrl = "/kepek/Termekek/karkoto.png",
                    Kategoria = "Ruházat",
                    Darabszam = 20
                },
                new Termek
                {
                    Nev = "Costa Serena sapka",
                    Leiras = "Stílusos napellenzős sapka hotel logóval.",
                    Ar = 4990,
                    KepUrl = "/kepek/Termekek/baseballsapka.png",
                    Kategoria = "Ruházat",
                    Darabszam = 20
                }
            };

            context.Termekek.AddRange(termekek);
            await context.SaveChangesAsync();
        }
    }
}