using costa_serena_grand_hotel_API.Data;
using costa_serena_grand_hotel_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace costa_serena_grand_hotel_API.Tests.TestHelpers;

public static class TestDbFactory
{
    public static HotelDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<HotelDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var context = new HotelDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}

public static class TestUserManagerFactory
{
    public static Mock<UserManager<IdentityUser>> Create()
    {
        var store = new Mock<IUserStore<IdentityUser>>();

        return new Mock<UserManager<IdentityUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }
}

public static class TestControllerHelper
{
    public static void SetUser(ControllerBase controller, string userId, string? email = null, params string[] roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim("sub", userId),
            new Claim("userid", userId)
        };

        if (!string.IsNullOrWhiteSpace(email))
        {
            claims.Add(new Claim("email", email));
            claims.Add(new Claim(ClaimTypes.Email, email));
            claims.Add(new Claim(ClaimTypes.Name, email));
        }

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }
}

public class FakeSession : ISession
{
    private readonly Dictionary<string, byte[]> _store = new();

    public IEnumerable<string> Keys => _store.Keys;
    public string Id { get; } = Guid.NewGuid().ToString();
    public bool IsAvailable => true;

    public void Clear() => _store.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => _store.Remove(key);

    public void Set(string key, byte[] value)
    {
        _store[key] = value;
    }

    public bool TryGetValue(string key, out byte[]? value)
    {
        return _store.TryGetValue(key, out value);
    }
}

public static class TestHttpContextFactory
{
    public static IHttpContextAccessor CreateAccessorWithSession()
    {
        var context = new DefaultHttpContext();
        context.Session = new FakeSession();

        return new HttpContextAccessor
        {
            HttpContext = context
        };
    }
}

public static class TestJwtFactory
{
    public static string CreateJwt(string userId, string email, params string[] roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TesztKulcsTesztKulcsTesztKulcs123456"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "test-issuer",
            audience: "test-audience",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public static class TestConfigurationFactory
{
    public static IConfiguration Create()
    {
        var values = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "TesztKulcsTesztKulcsTesztKulcs123456",
            ["Jwt:Issuer"] = "test-issuer",
            ["Jwt:Audience"] = "test-audience"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}

public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
        => new TestAsyncEnumerable<TEntity>(expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new TestAsyncEnumerable<TElement>(expression);

    public object Execute(Expression expression)
        => _inner.Execute(expression)!;

    public TResult Execute<TResult>(Expression expression)
        => _inner.Execute<TResult>(expression)!;

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var expectedResultType = typeof(TResult).GetGenericArguments().FirstOrDefault();
        var executionResult = expectedResultType == null
            ? _inner.Execute(expression)
            : typeof(IQueryProvider)
                .GetMethod(nameof(IQueryProvider.Execute), 1, new[] { typeof(Expression) })!
                .MakeGenericMethod(expectedResultType)
                .Invoke(_inner, new object[] { expression });

        return (TResult)typeof(Task)
            .GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(expectedResultType!)
            .Invoke(null, new[] { executionResult })!;
    }
}

public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
    {
    }

    public TestAsyncEnumerable(Expression expression) : base(expression)
    {
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
        => new ValueTask<bool>(_inner.MoveNext());
}