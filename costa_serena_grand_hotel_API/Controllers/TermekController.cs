using costa_serena_grand_hotel_API.Data;
using costa_serena_grand_hotel_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace costa_serena_grand_hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TermekController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public TermekController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Termek>>> GetAll()
        {
            var termekek = await _context.Termekek
                .Where(t => t.Darabszam > 0)
                .OrderBy(t => t.Nev)
                .ToListAsync();

            return Ok(termekek);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Termek>>> GetAllAdmin()
        {
            var termekek = await _context.Termekek
                .OrderBy(t => t.Id)
                .ToListAsync();

            return Ok(termekek);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Termek>> GetById(int id)
        {
            var termek = await _context.Termekek.FindAsync(id);

            if (termek == null)
                return NotFound("A termék nem található.");

            return Ok(termek);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Termek>> Create(Termek termek)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (termek.Darabszam < 0)
                return BadRequest("A darabszám nem lehet negatív.");

            _context.Termekek.Add(termek);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = termek.Id }, termek);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, Termek termek)
        {
            if (id != termek.Id)
                return BadRequest("Azonosító eltérés.");

            if (termek.Darabszam < 0)
                return BadRequest("A darabszám nem lehet negatív.");

            var meglevo = await _context.Termekek.FindAsync(id);
            if (meglevo == null)
                return NotFound("A termék nem található.");

            meglevo.Nev = termek.Nev;
            meglevo.Leiras = termek.Leiras;
            meglevo.Ar = termek.Ar;
            meglevo.KepUrl = termek.KepUrl;
            meglevo.Kategoria = termek.Kategoria;
            meglevo.Darabszam = termek.Darabszam;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var termek = await _context.Termekek.FindAsync(id);

            if (termek == null)
                return NotFound("A termék nem található.");

            var hasznalvaVan = await _context.RendelesTetelek
                .AnyAsync(x => x.TermekId == id);

            if (hasznalvaVan)
                return BadRequest("A termék nem törölhető, mert kapcsolódik meglévő rendeléshez.");

            try
            {
                _context.Termekek.Remove(termek);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}