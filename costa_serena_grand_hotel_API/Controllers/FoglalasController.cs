using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using costa_serena_grand_hotel_API.Data;
using costa_serena_grand_hotel_API.Models;

namespace costa_serena_grand_hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoglalasController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public FoglalasController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: api/Foglalas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Foglalas>>> GetFoglalasok()
        {
            return await _context.Foglalasok.ToListAsync();
        }

        // GET: api/Foglalas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Foglalas>> GetFoglalas(int id)
        {
            var foglalas = await _context.Foglalasok.FindAsync(id);

            if (foglalas == null)
            {
                return NotFound();
            }

            return foglalas;
        }

        // PUT: api/Foglalas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFoglalas(int id, Foglalas foglalas)
        {
            if (id != foglalas.Id)
            {
                return BadRequest();
            }

            _context.Entry(foglalas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoglalasExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Foglalas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Foglalas>> PostFoglalas(Foglalas foglalas)
        {
            _context.Foglalasok.Add(foglalas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFoglalas", new { id = foglalas.Id }, foglalas);
        }

        // DELETE: api/Foglalas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoglalas(int id)
        {
            var foglalas = await _context.Foglalasok.FindAsync(id);
            if (foglalas == null)
            {
                return NotFound();
            }

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
