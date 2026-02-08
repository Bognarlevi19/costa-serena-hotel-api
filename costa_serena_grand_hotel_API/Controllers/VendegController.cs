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
    public class VendegController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public VendegController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: api/Vendeg
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendeg>>> GetVendegek()
        {
            return await _context.Vendegek.ToListAsync();
        }

        // GET: api/Vendeg/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vendeg>> GetVendeg(int id)
        {
            var vendeg = await _context.Vendegek.FindAsync(id);

            if (vendeg == null)
            {
                return NotFound();
            }

            return vendeg;
        }

        // PUT: api/Vendeg/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendeg(int id, Vendeg vendeg)
        {
            if (id != vendeg.Id)
            {
                return BadRequest();
            }

            _context.Entry(vendeg).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendegExists(id))
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

        // POST: api/Vendeg
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vendeg>> PostVendeg(Vendeg vendeg)
        {
            _context.Vendegek.Add(vendeg);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVendeg", new { id = vendeg.Id }, vendeg);
        }

        // DELETE: api/Vendeg/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendeg(int id)
        {
            var vendeg = await _context.Vendegek.FindAsync(id);
            if (vendeg == null)
            {
                return NotFound();
            }

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
