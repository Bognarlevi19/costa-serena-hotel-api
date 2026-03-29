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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAll()
        {
            var rendelesek = await _context.Rendelesek
                .Include(r => r.Vendeg)
                .Include(r => r.Tetelek)
                .ThenInclude(t => t.Termek)
                .OrderByDescending(r => r.Letrehozva)
                .Select(r => new
                {
                    r.Id,
                    r.Nev,
                    r.Letrehozva,
                    r.Vegosszeg,
                    r.Fizetett,
                    TetelDb = r.Tetelek.Count,
                    Tetelek = r.Tetelek.Select(t => new
                    {
                        t.Id,
                        t.TermekId,
                        TermekNev = t.Termek.Nev,
                        t.Mennyiseg,
                        t.Egysegar,
                        t.Osszeg
                    })
                })
                .ToListAsync();

            return Ok(rendelesek);
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
                .Where(t => termekIds.Contains(t.Id) && t.Aktiv)
                .ToListAsync();

            if (termekek.Count != termekIds.Count)
                return BadRequest("Az egyik termék nem található vagy nem aktív.");

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
                Fizetett = false
            };

            foreach (var tetel in req.Tetelek)
            {
                var termek = termekek.First(x => x.Id == tetel.TermekId);
                var mennyiseg = tetel.Mennyiseg <= 0 ? 1 : tetel.Mennyiseg;

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
    }
}