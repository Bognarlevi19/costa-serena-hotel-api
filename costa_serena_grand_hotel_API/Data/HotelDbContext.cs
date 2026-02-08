using costa_serena_grand_hotel_API.Models;
using Microsoft.EntityFrameworkCore;
namespace costa_serena_grand_hotel_API.Data
{
    public class HotelDbContext:DbContext
    {
        public DbSet<Szoba> Szobak { get; set; } = null!;   
        public DbSet<Vendeg> Vendegek { get; set; } = null!;
        public DbSet<Foglalas> Foglalasok { get; set; } = null!;


        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }
    }
}
