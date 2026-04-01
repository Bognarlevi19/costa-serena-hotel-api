using costa_serena_grand_hotel_API.Data;
using costa_serena_grand_hotel_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace costa_serena_grand_hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SzobaKategoriaController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public SzobaKategoriaController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetSzobaKategoriak()
        {
            var kategoriak = await _context.SzobaKategoriak
                .OrderBy(k => k.Id)
                .Select(k => new
                {
                    k.Id,
                    k.Nev,
                    k.Leiras,
                    k.KepekJson,
                    k.Darab
                })
                .ToListAsync();

            return Ok(kategoriak);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetSzobaKategoria(int id)
        {
            var kategoria = await _context.SzobaKategoriak
                .Where(k => k.Id == id)
                .Select(k => new
                {
                    k.Id,
                    k.Nev,
                    k.Leiras,
                    k.KepekJson,
                    k.Darab
                })
                .FirstOrDefaultAsync();

            if (kategoria == null)
                return NotFound();

            return Ok(kategoria);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SzobaKategoria>> PostSzobaKategoria(SzobaKategoria kategoria)
        {
            _context.SzobaKategoriak.Add(kategoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSzobaKategoria), new { id = kategoria.Id }, kategoria);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutSzobaKategoria(int id, SzobaKategoria kategoria)
        {
            if (id != kategoria.Id)
                return BadRequest();

            var meglevo = await _context.SzobaKategoriak.FindAsync(id);
            if (meglevo == null)
                return NotFound();

            meglevo.Nev = kategoria.Nev;
            meglevo.Leiras = kategoria.Leiras;
            meglevo.KepekJson = kategoria.KepekJson;
            meglevo.Darab = kategoria.Darab;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSzobaKategoria(int id)
        {
            var kategoria = await _context.SzobaKategoriak.FindAsync(id);
            if (kategoria == null)
                return NotFound();

            var hasznaljak = await _context.Szobak.AnyAsync(x => x.SzobaKategoriaId == id);
            if (hasznaljak)
                return BadRequest("A szobakategória nem törölhető, mert tartoznak hozzá szobák.");

            _context.SzobaKategoriak.Remove(kategoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}