using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace costa_serena_grand_hotel_API.Models
{
    [Table("szoba")]
    public class Szoba
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A szobaszám megadása kötelező.")]
        [StringLength(5, ErrorMessage = "A szobaszám legfeljebb 5 karakter hosszú lehet.")]
        public string Szam { get; set; } = string.Empty;

        [Required(ErrorMessage = "Az emelet megadása kötelező.")]
        [Range(0, 20, ErrorMessage = "Az emelet értéke 0 és 20 között lehet.")]
        public int Emelet { get; set; }

        [Required(ErrorMessage = "Az alapterület megadása kötelező.")]
        [Range(1, 1000, ErrorMessage = "Az alapterület 1 és 1000 m² között lehet.")]
        public double Alapterulet { get; set; }

        [Required(ErrorMessage = "Az ár megadása kötelező.")]
        public int Ar { get; set; }

        [Required]
        [StringLength(120)]
        public string Nev { get; set; } = string.Empty;

        [StringLength(250)]
        public string? RovidLeiras { get; set; }

        public string? Leiras { get; set; }

        [Range(1, 20)]
        public int Ferohely { get; set; }

        [ForeignKey(nameof(SzobaKategoria))]
        public int SzobaKategoriaId { get; set; }

        public SzobaKategoria? SzobaKategoria { get; set; }

        public ICollection<SzobaKep> Kepek { get; set; } = new List<SzobaKep>();

        public ICollection<Foglalas> Foglalasok { get; set; } = new List<Foglalas>();
    }
}