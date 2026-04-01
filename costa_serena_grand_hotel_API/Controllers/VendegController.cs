using costa_serena_grand_hotel_API.Data;
using costa_serena_grand_hotel_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace costa_serena_grand_hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class VendegController : ControllerBase
    {
        private readonly HotelDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public VendegController(HotelDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Vendeg>>> GetVendegek()
        {
            return Ok(await _context.Vendegek
                .Select(v => new
                {
                    v.Id,
                    v.SzemelyiIgazolvanySzam,
                    v.Nev,
                    v.IranyitoSzam,
                    v.Varos,
                    v.Utca,
                    v.Hazszam,
                    v.IdentityUserId,
                    FoglalasokSzama = v.Foglalasok.Count
                })
                .ToListAsync());
        }

        [HttpGet("me")]
        public async Task<ActionResult<object>> GetCurrentVendeg()
        {
            var identityUserId =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(identityUserId))
                return Unauthorized();

            var vendeg = await _context.Vendegek
                .Where(v => v.IdentityUserId == identityUserId)
                .Select(v => new
                {
                    v.Id,
                    v.SzemelyiIgazolvanySzam,
                    v.Nev,
                    v.IranyitoSzam,
                    v.Varos,
                    v.Utca,
                    v.Hazszam,
                    v.IdentityUserId
                })
                .FirstOrDefaultAsync();

            if (vendeg == null)
                return NotFound();

            return Ok(vendeg);
        }

        [HttpPut("me")]
        public async Task<IActionResult> PutCurrentVendeg(Vendeg vendeg)
        {
            var identityUserId =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(identityUserId))
                return Unauthorized();

            var meglevoVendeg = await _context.Vendegek
                .FirstOrDefaultAsync(v => v.IdentityUserId == identityUserId);

            if (meglevoVendeg == null)
                return NotFound();

            meglevoVendeg.Nev = vendeg.Nev;
            meglevoVendeg.SzemelyiIgazolvanySzam = vendeg.SzemelyiIgazolvanySzam;
            meglevoVendeg.IranyitoSzam = vendeg.IranyitoSzam;
            meglevoVendeg.Varos = vendeg.Varos;
            meglevoVendeg.Utca = vendeg.Utca;
            meglevoVendeg.Hazszam = vendeg.Hazszam;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vendeg>> GetVendeg(int id)
        {
            var identityUserId =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(identityUserId))
                return Unauthorized();

            var vendeg = await _context.Vendegek.FirstOrDefaultAsync(v => v.Id == id);

            if (vendeg == null)
                return NotFound();

            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && vendeg.IdentityUserId != identityUserId)
                return Forbid();

            return Ok(vendeg);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutVendeg(int id, Vendeg vendeg)
        {
            if (id != vendeg.Id)
                return BadRequest();

            var meglevoVendeg = await _context.Vendegek.FirstOrDefaultAsync(v => v.Id == id);

            if (meglevoVendeg == null)
                return NotFound();

            meglevoVendeg.Nev = vendeg.Nev;
            meglevoVendeg.SzemelyiIgazolvanySzam = vendeg.SzemelyiIgazolvanySzam;
            meglevoVendeg.IranyitoSzam = vendeg.IranyitoSzam;
            meglevoVendeg.Varos = vendeg.Varos;
            meglevoVendeg.Utca = vendeg.Utca;
            meglevoVendeg.Hazszam = vendeg.Hazszam;

            if (!string.IsNullOrWhiteSpace(vendeg.IdentityUserId))
            {
                meglevoVendeg.IdentityUserId = vendeg.IdentityUserId;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Vendeg>> PostVendeg(Vendeg vendeg)
        {
            _context.Vendegek.Add(vendeg);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVendeg), new { id = vendeg.Id }, vendeg);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVendeg(int id)
        {
            var vendeg = await _context.Vendegek.FirstOrDefaultAsync(v => v.Id == id);
            if (vendeg == null)
                return NotFound();

            var vanFoglalasa = await _context.Foglalasok.AnyAsync(f => f.VendegId == id);
            if (vanFoglalasa)
                return BadRequest("A vendég nem törölhető, mert tartozik hozzá foglalás.");

            var vanRendelese = await _context.Rendelesek.AnyAsync(r => r.VendegId == id);
            if (vanRendelese)
                return BadRequest("A vendég nem törölhető, mert tartozik hozzá rendelés.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                IdentityUser? identityUser = null;

                if (!string.IsNullOrWhiteSpace(vendeg.IdentityUserId))
                {
                    identityUser = await _userManager.FindByIdAsync(vendeg.IdentityUserId);
                }

                _context.Vendegek.Remove(vendeg);
                await _context.SaveChangesAsync();

                if (identityUser != null)
                {
                    var deleteUserResult = await _userManager.DeleteAsync(identityUser);
                    if (!deleteUserResult.Succeeded)
                    {
                        var hibak = string.Join(" | ", deleteUserResult.Errors.Select(e => e.Description));
                        await transaction.RollbackAsync();
                        return BadRequest($"A vendég törlése közben a felhasználói fiók törlése nem sikerült: {hibak}");
                    }
                }

                await transaction.CommitAsync();
                return NoContent();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}