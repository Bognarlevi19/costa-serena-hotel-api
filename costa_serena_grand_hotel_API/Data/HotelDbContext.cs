using costa_serena_grand_hotel_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace costa_serena_grand_hotel_API.Data
{
    public class HotelDbContext : IdentityDbContext<IdentityUser>
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vendeg> Vendegek { get; set; }
        public DbSet<Szoba> Szobak { get; set; }
        public DbSet<Foglalas> Foglalasok { get; set; }
        public DbSet<Ertekeles> Ertekelesek { get; set; }
    }
}
