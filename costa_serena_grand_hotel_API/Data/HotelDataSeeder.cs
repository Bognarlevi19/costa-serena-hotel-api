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
                    Nev = "Basic Comfort",
                    Leiras = "Kényelmes, letisztult szoba alapfelszereltséggel, pihenésre tervezve.",
                    Darab = 10,
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/basic/basic1.png",
                        "/kepek/Szobak/basic/basic2.png",
                        "/kepek/Szobak/basic/basicfurdo.png"
                    })
                },
                new SzobaKategoria
                {
                    Nev = "Deluxe Family",
                    Leiras = "Tágas családi szoba több férőhellyel, barátságos elrendezéssel.",
                    Darab = 8,
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/deluxe_family/deluxe_family1.png",
                        "/kepek/Szobak/deluxe_family/deluxe_family2.png",
                        "/kepek/Szobak/deluxe_family/deluxe_family_furdo.png"
                    })
                },
                new SzobaKategoria
                {
                    Nev = "Luxury Ocean View",
                    Leiras = "Prémium szoba nagy erkéllyel és lenyűgöző óceáni panorámával.",
                    Darab = 6,
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/ocean_view/ocean_view1.png",
                        "/kepek/Szobak/ocean_view/ocean_view2.png",
                        "/kepek/Szobak/ocean_view/ocean_view_furdo.png"
                    })
                },
                new SzobaKategoria
                {
                    Nev = "Luxury Serenity",
                    Leiras = "Nyugodt hangulatú, kifinomult belsővel kialakított exkluzív szoba.",
                    Darab = 5,
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/serenity/serenity1.png",
                        "/kepek/Szobak/serenity/serenity2.png",
                        "/kepek/Szobak/serenity/serenity_furdo.png"
                    })
                },
                new SzobaKategoria
                {
                    Nev = "Luxury Signature",
                    Leiras = "Egyedi megjelenésű, magas kategóriás luxusszoba különleges enteriőrrel.",
                    Darab = 4,
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/signature/signature1.png",
                        "/kepek/Szobak/signature/signature2.png",
                        "/kepek/Szobak/signature/signature_furdo.png"
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
                new Szoba
                {
                    Szam = "101",
                    Emelet = 1,
                    Alapterulet = 24,
                    Ar = 24990,
                    Nev = "Basic Comfort 101",
                    RovidLeiras = "Kétágyas, kényelmes alap szoba.",
                    Leiras = "Letisztult berendezésű basic szoba két vendég részére, modern fürdőszobával.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Basic Comfort"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/basic/basic1.png",
                        "/kepek/Szobak/basic/basic2.png",
                        "/kepek/Szobak/basic/basicfurdo.png"
                    })
                },
                new Szoba
                {
                    Szam = "102",
                    Emelet = 1,
                    Alapterulet = 26,
                    Ar = 25990,
                    Nev = "Basic Comfort 102",
                    RovidLeiras = "Világos basic szoba két főnek.",
                    Leiras = "Praktikus kialakítású, barátságos hangulatú szoba erkéllyel.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Basic Comfort"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/basic/basic1.png",
                        "/kepek/Szobak/basic/basic2.png",
                        "/kepek/Szobak/basic/basicfurdo.png"
                    })
                },
                new Szoba
                {
                    Szam = "201",
                    Emelet = 2,
                    Alapterulet = 34,
                    Ar = 38990,
                    Nev = "Deluxe Family 201",
                    RovidLeiras = "Családi szoba több férőhellyel.",
                    Leiras = "Tágas deluxe family szoba családoknak, kényelmes fekvőhelyekkel.",
                    Ferohely = 4,
                    SzobaKategoriaId = kategoriak["Deluxe Family"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/deluxe_family/deluxe_family1.png",
                        "/kepek/Szobak/deluxe_family/deluxe_family2.png",
                        "/kepek/Szobak/deluxe_family/deluxe_family_furdo.png"
                    })
                },
                new Szoba
                {
                    Szam = "202",
                    Emelet = 2,
                    Alapterulet = 36,
                    Ar = 39990,
                    Nev = "Deluxe Family 202",
                    RovidLeiras = "Tágas családi lakosztály.",
                    Leiras = "Családbarát kialakítású deluxe szoba nagyobb alapterülettel.",
                    Ferohely = 4,
                    SzobaKategoriaId = kategoriak["Deluxe Family"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/deluxe_family/deluxe_family1.png",
                        "/kepek/Szobak/deluxe_family/deluxe_family2.png",
                        "/kepek/Szobak/deluxe_family/deluxe_family_furdo.png"
                    })
                },
                new Szoba
                {
                    Szam = "301",
                    Emelet = 3,
                    Alapterulet = 42,
                    Ar = 54990,
                    Nev = "Luxury Ocean View 301",
                    RovidLeiras = "Panorámás luxusszoba erkéllyel.",
                    Leiras = "Exkluzív szoba óceáni panorámával, nagy erkéllyel és prémium berendezéssel.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Luxury Ocean View"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/ocean_view/ocean_view1.png",
                        "/kepek/Szobak/ocean_view/ocean_view2.png",
                        "/kepek/Szobak/ocean_view/ocean_view_furdo.png"
                    })
                },
                new Szoba
                {
                    Szam = "302",
                    Emelet = 3,
                    Alapterulet = 44,
                    Ar = 56990,
                    Nev = "Luxury Ocean View 302",
                    RovidLeiras = "Magas kategóriás ocean view szoba.",
                    Leiras = "Elegáns, világos luxusszoba páratlan kilátással és nyugodt hangulattal.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Luxury Ocean View"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/ocean_view/ocean_view1.png",
                        "/kepek/Szobak/ocean_view/ocean_view2.png",
                        "/kepek/Szobak/ocean_view/ocean_view_furdo.png"
                    })
                },
                new Szoba
                {
                    Szam = "401",
                    Emelet = 4,
                    Alapterulet = 40,
                    Ar = 59990,
                    Nev = "Luxury Serenity 401",
                    RovidLeiras = "Nyugodt, exkluzív luxusszoba.",
                    Leiras = "Kifinomult részletekkel kialakított szoba a teljes kikapcsolódásért.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Luxury Serenity"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/serenity/serenity1.png",
                        "/kepek/Szobak/serenity/serenity2.png",
                        "/kepek/Szobak/serenity/serenity_furdo.png"
                    })
                },
                new Szoba
                {
                    Szam = "402",
                    Emelet = 4,
                    Alapterulet = 41,
                    Ar = 61990,
                    Nev = "Luxury Serenity 402",
                    RovidLeiras = "Prémium nyugalmat árasztó szoba.",
                    Leiras = "Elegáns és harmonikus enteriőr, visszafogott luxusérzettel.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Luxury Serenity"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/serenity/serenity1.png",
                        "/kepek/Szobak/serenity/serenity2.png",
                        "/kepek/Szobak/serenity/serenity_furdo.png"
                    })
                },
                new Szoba
                {
                    Szam = "501",
                    Emelet = 5,
                    Alapterulet = 48,
                    Ar = 69990,
                    Nev = "Luxury Signature 501",
                    RovidLeiras = "Különleges kialakítású signature szoba.",
                    Leiras = "Egyedi belsőépítészeti megoldásokkal berendezett, reprezentatív luxusszoba.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Luxury Signature"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/signature/signature1.png",
                        "/kepek/Szobak/signature/signature2.png",
                        "/kepek/Szobak/signature/signature_furdo.png"
                    })
                },
                new Szoba
                {
                    Szam = "502",
                    Emelet = 5,
                    Alapterulet = 50,
                    Ar = 72990,
                    Nev = "Luxury Signature 502",
                    RovidLeiras = "Látványos, felső kategóriás lakosztály.",
                    Leiras = "Tágas signature szoba kiemelt dizájnnal és exkluzív részletekkel.",
                    Ferohely = 2,
                    SzobaKategoriaId = kategoriak["Luxury Signature"],
                    KepekJson = JsonSerializer.Serialize(new List<string>
                    {
                        "/kepek/Szobak/signature/signature1.png",
                        "/kepek/Szobak/signature/signature2.png",
                        "/kepek/Szobak/signature/signature_furdo.png"
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
                    KepUrl = "/kepek/Shop/torolkozo.png",
                    Kategoria = "Textil",
                    Aktiv = true
                },
                new Termek
                {
                    Nev = "Costa Serena fürdőköpeny",
                    Leiras = "Prémium minőségű, fehér saját márkás fürdőköpeny.",
                    Ar = 12990,
                    KepUrl = "/kepek/Shop/furdokopeny.png",
                    Kategoria = "Textil",
                    Aktiv = true
                },
                new Termek
                {
                    Nev = "Costa Serena bögre",
                    Leiras = "Elegáns hotel logós porcelán bögre.",
                    Ar = 3990,
                    KepUrl = "/kepek/Shop/bogre.png",
                    Kategoria = "Ajándék",
                    Aktiv = true
                },
                new Termek
                {
                    Nev = "Costa Serena kulcstartó",
                    Leiras = "Kis méretű, fém hotel logós kulcstartó.",
                    Ar = 1990,
                    KepUrl = "/kepek/Shop/kulcstarto.png",
                    Kategoria = "Ajándék",
                    Aktiv = true
                },
                new Termek
                {
                    Nev = "Costa Serena póló",
                    Leiras = "Kényelmes, saját márkás pamut póló több méretben.",
                    Ar = 6990,
                    KepUrl = "/kepek/Shop/polo.png",
                    Kategoria = "Ruházat",
                    Aktiv = true
                },
                new Termek
                {
                    Nev = "Costa Serena sapka",
                    Leiras = "Stílusos napellenzős sapka hotel logóval.",
                    Ar = 4990,
                    KepUrl = "/kepek/Shop/sapka.png",
                    Kategoria = "Ruházat",
                    Aktiv = true
                }
            };

            context.Termekek.AddRange(termekek);
            await context.SaveChangesAsync();
        }
    }
}