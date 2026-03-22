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
    public class FoglalasController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public FoglalasController(HotelDbContext context)
        {
            _context = context;
        }

        public class FoglalasCreateRequest
        {
            public int SzobaId { get; set; }
            public DateTime Mettol { get; set; }
            public DateTime Meddig { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Foglalas>>> GetFoglalasok()
        {
            return await _context.Foglalasok.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetFoglalas(int id)
        {
            var foglalas = await _context.Foglalasok
                .Where(f => f.Id == id)
                .Select(f => new
                {
                    f.Id,
                    f.SzobaId,
                    f.VendegId,
                    f.Mettol,
                    f.Meddig,
                    f.Fizetett
                })
                .FirstOrDefaultAsync();

            if (foglalas == null)
                return NotFound();

            return Ok(foglalas);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutFoglalas(int id, Foglalas foglalas)
        {
            if (id != foglalas.Id)
                return BadRequest();

            _context.Entry(foglalas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoglalasExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Foglalas>> PostFoglalas(FoglalasCreateRequest request)
        {
            var identityUserId =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(identityUserId))
                return Unauthorized();

            var vendeg = await _context.Vendegek
                .FirstOrDefaultAsync(v => v.IdentityUserId == identityUserId);

            if (vendeg == null)
                return BadRequest("A bejelentkezett felhasználóhoz nem tartozik vendég rekord.");

            var szobaLetezik = await _context.Szobak.AnyAsync(s => s.Id == request.SzobaId);
            if (!szobaLetezik)
                return BadRequest("A kiválasztott szoba nem létezik.");

            if (request.Mettol.Date < DateTime.Today)
                return BadRequest("A foglalás kezdete nem lehet múltbeli dátum.");

            if (request.Meddig.Date <= request.Mettol.Date)
                return BadRequest("A távozás dátuma későbbi kell legyen, mint az érkezés dátuma.");

            var utkozik = await _context.Foglalasok.AnyAsync(f =>
                f.SzobaId == request.SzobaId &&
                request.Mettol < f.Meddig &&
                request.Meddig > f.Mettol);

            if (utkozik)
                return BadRequest("Erre az időszakra a szoba már foglalt.");

            var foglalas = new Foglalas
            {
                SzobaId = request.SzobaId,
                VendegId = vendeg.Id,
                Mettol = request.Mettol,
                Meddig = request.Meddig,
                Fizetett = false
            };

            _context.Foglalasok.Add(foglalas);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFoglalas), new { id = foglalas.Id }, new
            {
                foglalas.Id,
                foglalas.SzobaId,
                foglalas.VendegId,
                foglalas.Mettol,
                foglalas.Meddig,
                foglalas.Fizetett
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFoglalas(int id)
        {
            var foglalas = await _context.Foglalasok.FindAsync(id);
            if (foglalas == null)
                return NotFound();

            _context.Foglalasok.Remove(foglalas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FoglalasExists(int id)
        {
            return _context.Foglalasok.Any(e => e.Id == id);
        }
    }
}