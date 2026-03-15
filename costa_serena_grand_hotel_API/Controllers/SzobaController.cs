using costa_serena_grand_hotel_API.Data;
using costa_serena_grand_hotel_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace costa_serena_grand_hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SzobaController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public SzobaController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetSzobak()
        {
            var szobak = await _context.Szobak
                .Include(s => s.SzobaKategoria)
                .Select(sz => new
                {
                    sz.Id,
                    sz.Nev,
                    KategoriaNev = sz.SzobaKategoria != null ? sz.SzobaKategoria.Nev : "",
                    sz.RovidLeiras,
                    sz.Ar,
                    sz.Ferohely,
                    sz.Alapterulet
                })
                .ToListAsync();

            return Ok(szobak);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetSzoba(int id)
        {
            var szoba = await _context.Szobak
                .Include(s => s.SzobaKategoria)
                .Where(s => s.Id == id)
                .Select(sz => new
                {
                    sz.Id,
                    sz.Szam,
                    sz.Emelet,
                    sz.Nev,
                    sz.RovidLeiras,
                    sz.Leiras,
                    sz.Ar,
                    sz.Ferohely,
                    sz.Alapterulet,
                    sz.SzobaKategoriaId,
                    KategoriaNev = sz.SzobaKategoria != null ? sz.SzobaKategoria.Nev : ""
                })
                .FirstOrDefaultAsync();

            if (szoba == null)
                return NotFound();

            return Ok(szoba);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Szoba>> PostSzoba(Szoba szoba)
        {
            _context.Szobak.Add(szoba);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSzoba), new { id = szoba.Id }, szoba);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutSzoba(int id, Szoba szoba)
        {
            if (id != szoba.Id)
                return BadRequest();

            _context.Entry(szoba).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Szobak.Any(e => e.Id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSzoba(int id)
        {
            var szoba = await _context.Szobak.FindAsync(id);
            if (szoba == null)
                return NotFound();

            _context.Szobak.Remove(szoba);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}