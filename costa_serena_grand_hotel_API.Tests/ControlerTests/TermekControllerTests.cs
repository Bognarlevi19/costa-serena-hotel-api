using costa_serena_grand_hotel_API.Controllers;
using costa_serena_grand_hotel_API.Models;
using costa_serena_grand_hotel_API.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace costa_serena_grand_hotel_API.Tests.ControllerTests;

public class TermekControllerTests
{
    [Fact]
    public async Task Delete_WhenProductLinkedToOrder_ReturnsBadRequest()
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
            Hazszam = "1"
        });

        context.Termekek.Add(new Termek
        {
            Id = 1,
            Nev = "Fürdőköpeny",
            Ar = 5000,
            Darabszam = 10,
            Kategoria = "Hotel"
        });

        context.Rendelesek.Add(new Rendeles
        {
            Id = 1,
            VendegId = 1,
            Nev = "Teszt Vendeg",
            SzemelyiIgazolvanySzam = "AA111111",
            IranyitoSzam = 9300,
            Varos = "Csorna",
            Utca = "Fo utca",
            Hazszam = "1",
            Letrehozva = DateTime.UtcNow,
            Vegosszeg = 5000,
            Fizetett = false,
            Elkuldve = false
        });

        context.RendelesTetelek.Add(new RendelesTetel
        {
            Id = 1,
            RendelesId = 1,
            TermekId = 1,
            Mennyiseg = 1,
            Egysegar = 5000,
            Osszeg = 5000
        });

        await context.SaveChangesAsync();

        var controller = new TermekController(context);

        var result = await controller.Delete(1);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("A termék nem törölhető, mert kapcsolódik meglévő rendeléshez.", badRequest.Value);
    }
}