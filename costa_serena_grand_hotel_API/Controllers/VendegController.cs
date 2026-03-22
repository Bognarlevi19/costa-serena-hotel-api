using costa_serena_grand_hotel_API.Data;
using costa_serena_grand_hotel_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public VendegController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
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
            var vendeg = await _context.Vendegek.FindAsync(id);

            if (vendeg == null)
                return NotFound();

            return vendeg;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutVendeg(int id, Vendeg vendeg)
        {
            if (id != vendeg.Id)
                return BadRequest();

            _context.Entry(vendeg).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendegExists(id))
                    return NotFound();
                else
                    throw;
            }

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
            var vendeg = await _context.Vendegek.FindAsync(id);
            if (vendeg == null)
                return NotFound();

            _context.Vendegek.Remove(vendeg);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VendegExists(int id)
        {
            return _context.Vendegek.Any(e => e.Id == id);
        }
    }
}