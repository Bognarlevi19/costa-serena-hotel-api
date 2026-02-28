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
    public class SzobaController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public SzobaController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: api/Szobas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Szoba>>> GetSzobak()
        {


            return Ok(await _context.Szobak
               .Select(sz => new
               {
                   sz.Id,
                   sz.Szam,
                   sz.Emelet,
                   sz.Alapterulet,
                   sz.Ar,
                   Foglalva_darabszam = sz.Foglalasok.Count,
                   Foglalasok = sz.Foglalasok.ToArray()
               })
               .ToListAsync());
        }

        // GET: api/Szobas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Szoba>> GetSzoba(int id)
        {
            var szoba = await _context.Szobak.FindAsync(id);

            if (szoba == null)
            {
                return NotFound();
            }

            return szoba;
        }

        // PUT: api/Szobas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSzoba(int id, Szoba szoba)
        {
            if (id != szoba.Id)
            {
                return BadRequest();
            }

            _context.Entry(szoba).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SzobaExists(id))
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

        // POST: api/Szobas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Szoba>> PostSzoba(Szoba szoba)
        {
            _context.Szobak.Add(szoba);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSzoba", new { id = szoba.Id }, szoba);
        }

        // DELETE: api/Szobas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSzoba(int id)
        {
            var szoba = await _context.Szobak.FindAsync(id);
            if (szoba == null)
            {
                return NotFound();
            }

            _context.Szobak.Remove(szoba);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SzobaExists(int id)
        {
            return _context.Szobak.Any(e => e.Id == id);
        }
    }
}
