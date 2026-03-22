using costa_serena_grand_hotel_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using costa_serena_grand_hotel_API.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace costa_serena_grand_hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HotelDbContext _hotelContext;
        private readonly UserManager<IdentityUser> _users;
        private readonly IConfiguration _cfg;

        public AuthController(
            UserManager<IdentityUser> users,
            IConfiguration cfg,
            HotelDbContext hotelContext)
        {
            _users = users;
            _cfg = cfg;
            _hotelContext = hotelContext;
        }

        public record RegisterRequest(
            string Email,
            string Password,
            string Nev,
            string SzemelyiIgazolvanySzam,
            int IranyitoSzam,
            string Varos,
            string Utca,
            string Hazszam
        );

        public record LoginRequest(string Email, string Password);

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            var user = new IdentityUser
            {
                UserName = req.Email,
                Email = req.Email
            };

            var result = await _users.CreateAsync(user, req.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            // Automatikusan kapjon User szerepkört
            await _users.AddToRoleAsync(user, "User");

            var ujVendeg = new Vendeg
            {
                Nev = req.Nev,
                SzemelyiIgazolvanySzam = req.SzemelyiIgazolvanySzam,
                IranyitoSzam = req.IranyitoSzam,
                Varos = req.Varos,
                Utca = req.Utca,
                Hazszam = req.Hazszam,
                IdentityUserId = user.Id
            };

            _hotelContext.Vendegek.Add(ujVendeg);
            await _hotelContext.SaveChangesAsync();

            return Ok(new { userId = user.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var user = await _users.FindByEmailAsync(req.Email);
            if (user == null)
                return Unauthorized();

            var ok = await _users.CheckPasswordAsync(user, req.Password);
            if (!ok)
                return Unauthorized();

            var roles = await _users.GetRolesAsync(user);

            // Ha valamiért nincs role-ja, kapjon User-t
            if (!roles.Any())
            {
                await _users.AddToRoleAsync(user, "User");
                roles = await _users.GetRolesAsync(user);
            }

            var token = CreateJwt(user, roles);
            return Ok(new { token });
        }

        private string CreateJwt(IdentityUser user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}