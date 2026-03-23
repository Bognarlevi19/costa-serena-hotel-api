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
                .Where(t => t.Aktiv)
                .OrderBy(t => t.Nev)
                .ToListAsync();

            return Ok(termekek);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Termek>> GetById(int id)
        {
            var termek = await _context.Termekek.FindAsync(id);

            if (termek == null)
                return NotFound();

            return Ok(termek);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Termek>> Create(Termek termek)
        {
            _context.Termekek.Add(termek);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = termek.Id }, termek);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, Termek termek)
        {
            if (id != termek.Id)
                return BadRequest();

            _context.Entry(termek).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var termek = await _context.Termekek.FindAsync(id);

            if (termek == null)
                return NotFound();

            _context.Termekek.Remove(termek);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}