using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace costa_serena_grand_hotel_API.Models
{
    [Table("szoba")]
    public class Szoba
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A szobaszám megadása kötelező.")]
        [StringLength(5, ErrorMessage = "A szobaszám legfeljebb 5 karakter hosszú lehet.")]
        public string Szam { get; set; }
        [Required(ErrorMessage = "Az emelet megadása kötelező.")]
        [Range(0, 5, ErrorMessage = "Az emelet értéke 0 és 5 között lehet.")]
        public int Emelet { get; set; }
        [Required(ErrorMessage = "Az alapterület megadása kötelező.")]
        [Range(1, 1000, ErrorMessage = "Az alapterület 1 és 1000 m² között lehet.")]
        public double Alapterulet { get; set; }

        public ICollection<Foglalas> Foglalasok { get; set; } = new List<Foglalas>();

    }
}
