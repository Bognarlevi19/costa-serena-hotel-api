using costa_serena_grand_hotel_API.Controllers;
using costa_serena_grand_hotel_API.Models;
using costa_serena_grand_hotel_API.Tests.TestHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace costa_serena_grand_hotel_API.Tests.ControllerTests;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_ValidRequest_CreatesUserAndVendeg()
    {
        using var context = TestDbFactory.CreateContext(Guid.NewGuid().ToString());
        var userManagerMock = TestUserManagerFactory.Create();
        var config = TestConfigurationFactory.Create();

        IdentityUser? createdUser = null;

        userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<IdentityUser, string>((u, _) =>
            {
                u.Id = "user-1";
                createdUser = u;
            });

        userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        var controller = new AuthController(userManagerMock.Object, config, context);

        var request = new AuthController.RegisterRequest(
            Email: "teszt@example.com",
            Password: "Teszt123!",
            Nev: "Teszt Elek",
            SzemelyiIgazolvanySzam: "AA123456",
            IranyitoSzam: 9300,
            Varos: "Csorna",
            Utca: "Fo utca",
            Hazszam: "12"
        );

        var result = await controller.Register(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);

        var vendeg = context.Vendegek.SingleOrDefault(v => v.IdentityUserId == "user-1");
        Assert.NotNull(vendeg);
        Assert.Equal("Teszt Elek", vendeg.Nev);
        Assert.Equal("teszt@example.com", createdUser?.Email);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        using var context = TestDbFactory.CreateContext(Guid.NewGuid().ToString());
        var userManagerMock = TestUserManagerFactory.Create();
        var config = TestConfigurationFactory.Create();

        var user = new IdentityUser
        {
            Id = "user-2",
            Email = "rosszjelszo@example.com",
            UserName = "rosszjelszo@example.com"
        };

        userManagerMock
            .Setup(x => x.FindByEmailAsync("rosszjelszo@example.com"))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, "hibasjelszo"))
            .ReturnsAsync(false);

        var controller = new AuthController(userManagerMock.Object, config, context);

        var result = await controller.Login(new AuthController.LoginRequest("rosszjelszo@example.com", "hibasjelszo"));

        Assert.IsType<UnauthorizedResult>(result);
    }
}