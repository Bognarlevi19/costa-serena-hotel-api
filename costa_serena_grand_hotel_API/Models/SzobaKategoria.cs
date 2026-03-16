using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace costa_serena_grand_hotel_API.Models
{
    [Table("szoba_kategoria")]
    public class SzobaKategoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(80)]
        public string Nev { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Leiras { get; set; }

        [Column(TypeName = "json")]
        public string? KepekJson { get; set; }

        public int Darab { get; set; } = 10;

        public ICollection<Szoba> Szobak { get; set; } = new List<Szoba>();
    }
}