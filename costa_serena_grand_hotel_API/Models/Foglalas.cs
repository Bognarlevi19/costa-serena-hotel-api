using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace costa_serena_grand_hotel_API.Models
{
    [Table("foglalas")]
    public class Foglalas
    {
        [Key]
        public int Id { get; set; } 

        public int SzobaId { get; set; }
        public Szoba Szoba { get; set; } = null!;

        public int VendegId { get; set; }
        public Vendeg Vendeg { get; set; } = null!;

        public DateTime Mettol { get; set; }
        public DateTime Meddig { get; set; }

        public int VegOsszeg { get; set; }
        public bool Fizetett { get; set; }
    }
}
