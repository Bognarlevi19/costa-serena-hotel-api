using System.Collections;
using costa_serena_grand_hotel_API.Controllers;
using costa_serena_grand_hotel_API.Models;
using costa_serena_grand_hotel_API.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace costa_serena_grand_hotel_API.Tests.ControllerTests;

public class FoglalasControllerTests
{
    [Fact]
    public async Task PostFoglalas_OverlappingDates_ReturnsBadRequest()
    {
        using var context = TestDbFactory.CreateContext(Guid.NewGuid().ToString());

        var vendeg = new Vendeg
        {
            Id = 1,
            Nev = "Teszt Vendeg",
            SzemelyiIgazolvanySzam = "AA111111",
            IranyitoSzam = 9300,
            Varos = "Csorna",
            Utca = "Fo utca",
            Hazszam = "1",
            IdentityUserId = "user-1",
        };

        var szoba = new Szoba
        {
            Id = 1,
            Szam = "101",
            Emelet = 1,
            Alapterulet = 25,
            Ar = 30000,
            Nev = "Deluxe szoba",
            Ferohely = 2,
            SzobaKategoriaId = 1
        };

        context.Vendegek.Add(vendeg);
        context.Szobak.Add(szoba);
        context.Foglalasok.Add(new Foglalas
        {
            Id = 1,
            SzobaId = 1,
            VendegId = 1,
            Mettol = new DateTime(2026, 4, 10),
            Meddig = new DateTime(2026, 4, 15),
            Fizetett = false
        });

        await context.SaveChangesAsync();

        var controller = new FoglalasController(context);
        TestControllerHelper.SetUser(controller, "user-1", "user1@example.com", "User");

        var request = new FoglalasController.FoglalasCreateRequest
        {
            SzobaId = 1,
            Mettol = new DateTime(2026, 4, 12),
            Meddig = new DateTime(2026, 4, 18)
        };

        var result = await controller.PostFoglalas(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Erre az időszakra a szoba már foglalt.", badRequest.Value);
    }

    [Fact]
    public async Task GetSajatFoglalasok_ReturnsOkResult()
    {
        using var context = TestDbFactory.CreateContext(Guid.NewGuid().ToString());

        var user1 = new Vendeg
        {
            Id = 1,
            Nev = "Elso Vendeg",
            SzemelyiIgazolvanySzam = "AA111111",
            IranyitoSzam = 9300,
            Varos = "Csorna",
            Utca = "Fo utca",
            Hazszam = "1",
            IdentityUserId = "user-1"
        };

        var user2 = new Vendeg
        {
            Id = 2,
            Nev = "Masodik Vendeg",
            SzemelyiIgazolvanySzam = "BB222222",
            IranyitoSzam = 9400,
            Varos = "Sopron",
            Utca = "Kis utca",
            Hazszam = "2",
            IdentityUserId = "user-2"
        };

        var szoba1 = new Szoba
        {
            Id = 1,
            Szam = "101",
            Emelet = 1,
            Alapterulet = 25,
            Ar = 30000,
            Nev = "Deluxe szoba",
            Ferohely = 2,
            SzobaKategoriaId = 1
        };

        var szoba2 = new Szoba
        {
            Id = 2,
            Szam = "102",
            Emelet = 1,
            Alapterulet = 20,
            Ar = 25000,
            Nev = "Basic szoba",
            Ferohely = 2,
            SzobaKategoriaId = 1
        };

        context.Vendegek.AddRange(user1, user2);
        context.Szobak.AddRange(szoba1, szoba2);
        context.Foglalasok.AddRange(
            new Foglalas
            {
                Id = 1,
                SzobaId = 1,
                VendegId = 1,
                Mettol = new DateTime(2026, 5, 1),
                Meddig = new DateTime(2026, 5, 5),
                Fizetett = false
            },
            new Foglalas
            {
                Id = 2,
                SzobaId = 2,
                VendegId = 2,
                Mettol = new DateTime(2026, 5, 10),
                Meddig = new DateTime(2026, 5, 12),
                Fizetett = false
            });

        await context.SaveChangesAsync();

        var controller = new FoglalasController(context);
        TestControllerHelper.SetUser(controller, "user-1", "user1@example.com", "User");

        var result = await controller.GetSajatFoglalasok();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);

        var enumerable = okResult.Value as IEnumerable;
        Assert.NotNull(enumerable);
    }
}