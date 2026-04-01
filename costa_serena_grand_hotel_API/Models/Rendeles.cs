using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace costa_serena_grand_hotel_API.Models
{
    [Table("rendeles")]
    public class Rendeles
    {
        [Key]
        public int Id { get; set; }

        public int VendegId { get; set; }
        public Vendeg Vendeg { get; set; } = null!;

        [Required]
        [StringLength(120)]
        public string Nev { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string SzemelyiIgazolvanySzam { get; set; } = string.Empty;

        [Required]
        public int IranyitoSzam { get; set; }

        [Required]
        [StringLength(30)]
        public string Varos { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Utca { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Hazszam { get; set; } = string.Empty;

        public DateTime Letrehozva { get; set; } = DateTime.UtcNow;

        public int Vegosszeg { get; set; }

        public bool Fizetett { get; set; }

        public bool Elkuldve { get; set; } = false;

        public ICollection<RendelesTetel> Tetelek { get; set; } = new List<RendelesTetel>();
    }
}