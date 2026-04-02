using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace costa_serena_grand_hotel_API.Models
{
    [Table("termek")]
    public class Termek
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Nev { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Leiras { get; set; }

        [Required]
        public int Ar { get; set; }

        [StringLength(255)]
        public string? KepUrl { get; set; }

        [StringLength(80)]
        public string? Kategoria { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "A darabszám nem lehet negatív.")]
        public int Darabszam { get; set; }
    }
}