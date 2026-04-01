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

        public class FoglalasAdminCreateRequest
        {
            public int SzobaId { get; set; }
            public int VendegId { get; set; }
            public DateTime Mettol { get; set; }
            public DateTime Meddig { get; set; }
            public bool Fizetett { get; set; }
        }

        public class SajatFoglalasDto
        {
            public int Id { get; set; }
            public int SzobaId { get; set; }
            public string SzobaSzam { get; set; } = string.Empty;
            public string SzobaNev { get; set; } = string.Empty;
            public string? KategoriaNev { get; set; }
            public DateTime Mettol { get; set; }
            public DateTime Meddig { get; set; }
            public bool Fizetett { get; set; }
        }
        public class FoglaltIdoszakDto
        {
            public DateTime Mettol { get; set; }
            public DateTime Meddig { get; set; }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetFoglalasok()
        {
            var foglalasok = await _context.Foglalasok
                .Include(f => f.Szoba)
                .Include(f => f.Vendeg)
                .OrderByDescending(f => f.Mettol)
                .Select(f => new
                {
                    f.Id,
                    f.SzobaId,
                    SzobaNev = f.Szoba.Nev,
                    SzobaSzam = f.Szoba.Szam,
                    f.VendegId,
                    VendegNev = f.Vendeg.Nev,
                    f.Mettol,
                    f.Meddig,
                    f.Fizetett
                })
                .ToListAsync();

            return Ok(foglalasok);
        }

        [HttpGet("sajat")]
        public async Task<ActionResult<IEnumerable<SajatFoglalasDto>>> GetSajatFoglalasok()
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

            var foglalasok = await _context.Foglalasok
                .Where(f => f.VendegId == vendeg.Id)
                .Include(f => f.Szoba)
                .ThenInclude(sz => sz.SzobaKategoria)
                .OrderByDescending(f => f.Mettol)
                .Select(f => new SajatFoglalasDto
                {
                    Id = f.Id,
                    SzobaId = f.SzobaId,
                    SzobaSzam = f.Szoba.Szam,
                    SzobaNev = f.Szoba.Nev,
                    KategoriaNev = f.Szoba.SzobaKategoria != null ? f.Szoba.SzobaKategoria.Nev : null,
                    Mettol = f.Mettol,
                    Meddig = f.Meddig,
                    Fizetett = f.Fizetett
                })
                .ToListAsync();

            return Ok(foglalasok);
        }

        [HttpGet("szoba/{szobaId:int}/foglalt-idoszakok")]
        public async Task<ActionResult<IEnumerable<FoglaltIdoszakDto>>> GetFoglaltIdoszakok(int szobaId)
        {
            var szobaLetezik = await _context.Szobak.AnyAsync(s => s.Id == szobaId);
            if (!szobaLetezik)
                return NotFound("A kiválasztott szoba nem található.");

            var ma = DateTime.Today;

            var idoszakok = await _context.Foglalasok
                .Where(f => f.SzobaId == szobaId && f.Meddig > ma)
                .OrderBy(f => f.Mettol)
                .Select(f => new FoglaltIdoszakDto
                {
                    Mettol = f.Mettol,
                    Meddig = f.Meddig
                })
                .ToListAsync();

            return Ok(idoszakok);
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
                return NotFound("A foglalás nem található.");

            return Ok(foglalas);
        }

        [HttpPost]
        public async Task<ActionResult> PostFoglalas(FoglalasCreateRequest request)
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

            return await CreateFoglalasInternal(request.SzobaId, vendeg.Id, request.Mettol, request.Meddig, false);
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PostAdminFoglalas(FoglalasAdminCreateRequest request)
        {
            return await CreateFoglalasInternal(request.SzobaId, request.VendegId, request.Mettol, request.Meddig, request.Fizetett);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutFoglalas(int id, Foglalas foglalas)
        {
            if (id != foglalas.Id)
                return BadRequest("Azonosító eltérés.");

            var meglevo = await _context.Foglalasok.FindAsync(id);
            if (meglevo == null)
                return NotFound("A foglalás nem található.");

            var szobaLetezik = await _context.Szobak.AnyAsync(s => s.Id == foglalas.SzobaId);
            if (!szobaLetezik)
                return BadRequest("A kiválasztott szoba nem létezik.");

            var vendegLetezik = await _context.Vendegek.AnyAsync(v => v.Id == foglalas.VendegId);
            if (!vendegLetezik)
                return BadRequest("A kiválasztott vendég nem létezik.");

            if (foglalas.Meddig.Date <= foglalas.Mettol.Date)
                return BadRequest("A távozás dátuma későbbi kell legyen, mint az érkezés dátuma.");

            var utkozik = await _context.Foglalasok.AnyAsync(f =>
                f.Id != id &&
                f.SzobaId == foglalas.SzobaId &&
                foglalas.Mettol < f.Meddig &&
                foglalas.Meddig > f.Mettol);

            if (utkozik)
                return BadRequest("Erre az időszakra a szoba már foglalt.");

            meglevo.SzobaId = foglalas.SzobaId;
            meglevo.VendegId = foglalas.VendegId;
            meglevo.Mettol = foglalas.Mettol;
            meglevo.Meddig = foglalas.Meddig;
            meglevo.Fizetett = foglalas.Fizetett;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFoglalas(int id)
        {
            var foglalas = await _context.Foglalasok.FindAsync(id);
            if (foglalas == null)
                return NotFound("A foglalás nem található.");

            _context.Foglalasok.Remove(foglalas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<ActionResult> CreateFoglalasInternal(int szobaId, int vendegId, DateTime mettol, DateTime meddig, bool fizetett)
        {
            var szoba = await _context.Szobak.FirstOrDefaultAsync(s => s.Id == szobaId);
            if (szoba == null)
                return BadRequest("A kiválasztott szoba nem létezik.");

            var vendeg = await _context.Vendegek.FirstOrDefaultAsync(v => v.Id == vendegId);
            if (vendeg == null)
                return BadRequest("A kiválasztott vendég nem létezik.");

            if (meddig.Date <= mettol.Date)
                return BadRequest("A távozás dátuma későbbi kell legyen, mint az érkezés dátuma.");

            var utkozik = await _context.Foglalasok.AnyAsync(f =>
                f.SzobaId == szobaId &&
                mettol < f.Meddig &&
                meddig > f.Mettol);

            if (utkozik)
                return BadRequest("Erre az időszakra a szoba már foglalt.");

            var ejszakakSzama = (meddig.Date - mettol.Date).Days;
            var fizetendoOsszeg = ejszakakSzama * szoba.Ar;

            var foglalas = new Foglalas
            {
                SzobaId = szobaId,
                VendegId = vendegId,
                Mettol = mettol,
                Meddig = meddig,
                Fizetett = fizetett
            };

            _context.Foglalasok.Add(foglalas);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Id = foglalas.Id,
                FizetendoOsszeg = fizetendoOsszeg,
                Uzenet = "A foglalás sikeresen mentve."
            });
        }
    }
}