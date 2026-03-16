using costa_serena_grand_hotel_API.Data;
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
    }
}