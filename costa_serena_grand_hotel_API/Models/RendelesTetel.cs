using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace costa_serena_grand_hotel_API.Models
{
    [Table("rendeles_tetel")]
    public class RendelesTetel
    {
        [Key]
        public int Id { get; set; }

        public int RendelesId { get; set; }
        public Rendeles Rendeles { get; set; } = null!;

        public int TermekId { get; set; }
        public Termek Termek { get; set; } = null!;

        public int Mennyiseg { get; set; }

        public int Egysegar { get; set; }

        public int Osszeg { get; set; }
    }
}