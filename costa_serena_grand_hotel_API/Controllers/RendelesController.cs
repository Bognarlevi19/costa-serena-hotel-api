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
    public class RendelesController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public RendelesController(HotelDbContext context)
        {
            _context = context;
        }

        public class RendelesTetelRequest
        {
            public int TermekId { get; set; }
            public int Mennyiseg { get; set; }
        }

        public class RendelesCreateRequest
        {
            public string Nev { get; set; } = string.Empty;
            public string SzemelyiIgazolvanySzam { get; set; } = string.Empty;
            public int IranyitoSzam { get; set; }
            public string Varos { get; set; } = string.Empty;
            public string Utca { get; set; } = string.Empty;
            public string Hazszam { get; set; } = string.Empty;
            public List<RendelesTetelRequest> Tetelek { get; set; } = new();
        }

        public class RendelesAdminCreateRequest
        {
            public int VendegId { get; set; }
            public string Nev { get; set; } = string.Empty;
            public string SzemelyiIgazolvanySzam { get; set; } = string.Empty;
            public int IranyitoSzam { get; set; }
            public string Varos { get; set; } = string.Empty;
            public string Utca { get; set; } = string.Empty;
            public string Hazszam { get; set; } = string.Empty;
            public int Vegosszeg { get; set; }
            public bool Fizetett { get; set; }
            public bool Elkuldve { get; set; }
        }

        public class RendelesAdminUpdateRequest
        {
            public int Id { get; set; }
            public int VendegId { get; set; }
            public string Nev { get; set; } = string.Empty;
            public string SzemelyiIgazolvanySzam { get; set; } = string.Empty;
            public int IranyitoSzam { get; set; }
            public string Varos { get; set; } = string.Empty;
            public string Utca { get; set; } = string.Empty;
            public string Hazszam { get; set; } = string.Empty;
            public int Vegosszeg { get; set; }
            public bool Fizetett { get; set; }
            public bool Elkuldve { get; set; }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAll()
        {
            var rendelesek = await _context.Rendelesek
                .Include(r => r.Tetelek)
                .OrderByDescending(r => r.Letrehozva)
                .Select(r => new
                {
                    r.Id,
                    r.VendegId,
                    r.Nev,
                    r.SzemelyiIgazolvanySzam,
                    r.IranyitoSzam,
                    r.Varos,
                    r.Utca,
                    r.Hazszam,
                    r.Letrehozva,
                    r.Vegosszeg,
                    r.Fizetett,
                    r.Elkuldve,
                    TetelDb = r.Tetelek.Count
                })
                .ToListAsync();

            return Ok(rendelesek);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetById(int id)
        {
            var rendeles = await _context.Rendelesek
                .Include(r => r.Tetelek)
                .Where(r => r.Id == id)
                .Select(r => new
                {
                    r.Id,
                    r.VendegId,
                    r.Nev,
                    r.SzemelyiIgazolvanySzam,
                    r.IranyitoSzam,
                    r.Varos,
                    r.Utca,
                    r.Hazszam,
                    r.Letrehozva,
                    r.Vegosszeg,
                    r.Fizetett,
                    r.Elkuldve,
                    TetelDb = r.Tetelek.Count
                })
                .FirstOrDefaultAsync();

            if (rendeles == null)
                return NotFound("A rendelés nem található.");

            return Ok(rendeles);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RendelesCreateRequest req)
        {
            var identityUserId =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(identityUserId))
                return Unauthorized();

            var vendeg = await _context.Vendegek
                .FirstOrDefaultAsync(v => v.IdentityUserId == identityUserId);

            if (vendeg == null)
                return BadRequest("Nem található a bejelentkezett felhasználóhoz tartozó vendég.");

            if (req.Tetelek == null || !req.Tetelek.Any())
                return BadRequest("A kosár üres.");

            vendeg.Nev = req.Nev;
            vendeg.SzemelyiIgazolvanySzam = req.SzemelyiIgazolvanySzam;
            vendeg.IranyitoSzam = req.IranyitoSzam;
            vendeg.Varos = req.Varos;
            vendeg.Utca = req.Utca;
            vendeg.Hazszam = req.Hazszam;

            var termekIds = req.Tetelek.Select(t => t.TermekId).Distinct().ToList();

            var termekek = await _context.Termekek
                .Where(t => termekIds.Contains(t.Id))
                .ToListAsync();

            if (termekek.Count != termekIds.Count)
                return BadRequest("Az egyik termék nem található.");

            var rendeles = new Rendeles
            {
                VendegId = vendeg.Id,
                Nev = req.Nev,
                SzemelyiIgazolvanySzam = req.SzemelyiIgazolvanySzam,
                IranyitoSzam = req.IranyitoSzam,
                Varos = req.Varos,
                Utca = req.Utca,
                Hazszam = req.Hazszam,
                Letrehozva = DateTime.UtcNow,
                Fizetett = false,
                Elkuldve = false
            };

            foreach (var tetel in req.Tetelek)
            {
                var termek = termekek.First(x => x.Id == tetel.TermekId);
                var mennyiseg = tetel.Mennyiseg <= 0 ? 1 : tetel.Mennyiseg;

                if (termek.Darabszam < mennyiseg)
                    return BadRequest($"Nincs elég készlet a következő termékből: {termek.Nev}. Elérhető darabszám: {termek.Darabszam}.");

                termek.Darabszam -= mennyiseg;

                rendeles.Tetelek.Add(new RendelesTetel
                {
                    TermekId = termek.Id,
                    Mennyiseg = mennyiseg,
                    Egysegar = termek.Ar,
                    Osszeg = termek.Ar * mennyiseg
                });
            }

            rendeles.Vegosszeg = rendeles.Tetelek.Sum(t => t.Osszeg);

            _context.Rendelesek.Add(rendeles);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                rendeles.Id,
                rendeles.Vegosszeg
            });
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdmin(RendelesAdminCreateRequest req)
        {
            var vendegLetezik = await _context.Vendegek.AnyAsync(v => v.Id == req.VendegId);
            if (!vendegLetezik)
                return BadRequest("A kiválasztott vendég nem létezik.");

            var rendeles = new Rendeles
            {
                VendegId = req.VendegId,
                Nev = req.Nev,
                SzemelyiIgazolvanySzam = req.SzemelyiIgazolvanySzam,
                IranyitoSzam = req.IranyitoSzam,
                Varos = req.Varos,
                Utca = req.Utca,
                Hazszam = req.Hazszam,
                Vegosszeg = req.Vegosszeg,
                Fizetett = req.Fizetett,
                Elkuldve = req.Elkuldve,
                Letrehozva = DateTime.UtcNow
            };

            _context.Rendelesek.Add(rendeles);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                rendeles.Id,
                rendeles.Vegosszeg
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, RendelesAdminUpdateRequest req)
        {
            if (id != req.Id)
                return BadRequest("Azonosító eltérés.");

            var rendeles = await _context.Rendelesek.FindAsync(id);
            if (rendeles == null)
                return NotFound("A rendelés nem található.");

            var vendegLetezik = await _context.Vendegek.AnyAsync(v => v.Id == req.VendegId);
            if (!vendegLetezik)
                return BadRequest("A kiválasztott vendég nem létezik.");

            rendeles.VendegId = req.VendegId;
            rendeles.Nev = req.Nev;
            rendeles.SzemelyiIgazolvanySzam = req.SzemelyiIgazolvanySzam;
            rendeles.IranyitoSzam = req.IranyitoSzam;
            rendeles.Varos = req.Varos;
            rendeles.Utca = req.Utca;
            rendeles.Hazszam = req.Hazszam;
            rendeles.Vegosszeg = req.Vegosszeg;
            rendeles.Fizetett = req.Fizetett;
            rendeles.Elkuldve = req.Elkuldve;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}/elkuldve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetElkuldve(int id, [FromBody] bool elkuldve)
        {
            var rendeles = await _context.Rendelesek.FindAsync(id);
            if (rendeles == null)
                return NotFound("A rendelés nem található.");

            rendeles.Elkuldve = elkuldve;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var rendeles = await _context.Rendelesek
                .Include(r => r.Tetelek)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rendeles == null)
                return NotFound("A rendelés nem található.");

            _context.RendelesTetelek.RemoveRange(rendeles.Tetelek);
            _context.Rendelesek.Remove(rendeles);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}