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
        public DbSet<SzobaKategoria> SzobaKategoriak { get; set; }
        public DbSet<Foglalas> Foglalasok { get; set; }
        public DbSet<Ertekeles> Ertekelesek { get; set; }
        public DbSet<Termek> Termekek { get; set; }
        public DbSet<Rendeles> Rendelesek { get; set; }
        public DbSet<RendelesTetel> RendelesTetelek { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Szoba>()
                .HasOne(s => s.SzobaKategoria)
                .WithMany(k => k.Szobak)
                .HasForeignKey(s => s.SzobaKategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RendelesTetel>()
                .HasOne(rt => rt.Rendeles)
                .WithMany(r => r.Tetelek)
                .HasForeignKey(rt => rt.RendelesId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RendelesTetel>()
                .HasOne(rt => rt.Termek)
                .WithMany()
                .HasForeignKey(rt => rt.TermekId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rendeles>()
                .HasOne(r => r.Vendeg)
                .WithMany()
                .HasForeignKey(r => r.VendegId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}