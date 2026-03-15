using costa_serena_grand_hotel_API.Data;
using costa_serena_grand_hotel_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace costa_serena_grand_hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErtekelesekController : ControllerBase
    {
        private readonly HotelDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ErtekelesekController(HotelDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Ertekeles>>> GetAll()
        {
            var ertekelesek = await _context.Ertekelesek
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(ertekelesek);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Ertekeles>> Create(CreateErtekelesRequest req)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(ClaimTypes.Name)
                         ?? User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Nem található a bejelentkezett felhasználó azonosítója.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized("A bejelentkezett felhasználó nem található.");

            var vendeg = await _context.Vendegek
                .FirstOrDefaultAsync(v => v.IdentityUserId == user.Id);

            var nev = vendeg?.Nev?.Trim();

            var ertekeles = new Ertekeles
            {
                Email = user.Email ?? string.Empty,
                Nev = string.IsNullOrWhiteSpace(nev) ? "Vendég" : nev,
                Rating = req.Rating,
                Comment = req.Comment.Trim(),
                CreatedAt = DateTime.Now,
                IdentityUserId = user.Id
            };

            _context.Ertekelesek.Add(ertekeles);
            await _context.SaveChangesAsync();

            return Ok(ertekeles);
        }

        public record CreateErtekelesRequest(
            int Rating,
            string Comment
        );
    }
}