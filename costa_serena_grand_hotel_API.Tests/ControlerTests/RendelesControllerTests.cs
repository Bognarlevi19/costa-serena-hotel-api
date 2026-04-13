using costa_serena_grand_hotel_API.Controllers;
using costa_serena_grand_hotel_API.Models;
using costa_serena_grand_hotel_API.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace costa_serena_grand_hotel_API.Tests.ControllerTests;

public class RendelesControllerTests
{
    [Fact]
    public async Task Create_WhenStockInsufficient_ReturnsBadRequest()
    {
        using var context = TestDbFactory.CreateContext(Guid.NewGuid().ToString());

        context.Vendegek.Add(new Vendeg
        {
            Id = 1,
            Nev = "Teszt Vendeg",
            SzemelyiIgazolvanySzam = "AA111111",
            IranyitoSzam = 9300,
            Varos = "Csorna",
            Utca = "Fo utca",
            Hazszam = "1",
            IdentityUserId = "user-1"
        });

        context.Termekek.Add(new Termek
        {
            Id = 1,
            Nev = "Sampon",
            Ar = 2000,
            Darabszam = 1,
            Kategoria = "Wellness"
        });

        await context.SaveChangesAsync();

        var controller = new RendelesController(context);
        TestControllerHelper.SetUser(controller, "user-1", "user1@example.com", "User");

        var request = new RendelesController.RendelesCreateRequest
        {
            Nev = "Teszt Vendeg",
            SzemelyiIgazolvanySzam = "AA111111",
            IranyitoSzam = 9300,
            Varos = "Csorna",
            Utca = "Fo utca",
            Hazszam = "1",
            Tetelek = new List<RendelesController.RendelesTetelRequest>
            {
                new RendelesController.RendelesTetelRequest
                {
                    TermekId = 1,
                    Mennyiseg = 3
                }
            }
        };

        var result = await controller.Create(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Nincs elég készlet", badRequest.Value?.ToString());
    }
}