using costa_serena_grand_hotel_API.Controllers;
using costa_serena_grand_hotel_API.Models;
using costa_serena_grand_hotel_API.Tests.TestHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace costa_serena_grand_hotel_API.Tests.ControllerTests;

public class AdminControllerTests
{
    [Fact]
    public async Task GetStats_ReturnsResultObject()
    {
        using var context = TestDbFactory.CreateContext(Guid.NewGuid().ToString());

        context.Logs.AddRange(
            new Log
            {
                Id = 1,
                Timestamp = DateTime.UtcNow,
                UserId = "user-1",
                UserEmail = "user1@example.com",
                HttpMethod = "GET",
                Path = "/api/admin/stats",
                StatusCode = 200,
                IsAuthFailure = false,
                EntityType = "Rendeles",
                Action = "Read"
            },
            new Log
            {
                Id = 2,
                Timestamp = DateTime.UtcNow,
                UserId = "user-2",
                UserEmail = "user2@example.com",
                HttpMethod = "POST",
                Path = "/api/auth/login",
                StatusCode = 401,
                IsAuthFailure = true,
                EntityType = "Auth",
                Action = "Login"
            });

        await context.SaveChangesAsync();

        var userManagerMock = TestUserManagerFactory.Create();

        var users = new List<IdentityUser>
        {
            new IdentityUser { Id = "user-1", Email = "user1@example.com", UserName = "user1@example.com" },
            new IdentityUser { Id = "user-2", Email = "user2@example.com", UserName = "user2@example.com" }
        };

        var asyncUsers = new TestAsyncEnumerable<IdentityUser>(users);

        userManagerMock
            .Setup(x => x.Users)
            .Returns(asyncUsers);

        var controller = new AdminController(userManagerMock.Object, context);

        var result = await controller.GetStats();

        var actionResult = result.Result;
        Assert.NotNull(actionResult);

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okResult.Value);
    }
}