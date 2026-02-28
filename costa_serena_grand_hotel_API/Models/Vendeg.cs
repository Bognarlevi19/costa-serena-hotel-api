using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace costa_serena_grand_hotel_API.Models
{
    [Table("vendeg")]
    public class Vendeg
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "A személyi igazolvány szám megadása kötelező.")]
        [StringLength(20, ErrorMessage = "A személyi igazolvány szám legfeljebb 20 karakter lehet.")]
        public string SzemelyiIgazolvanySzam { get; set; } = String.Empty;


        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(100, ErrorMessage = "A név legfeljebb 100 karakter lehet.")]
        public string Nev { get; set; } = String.Empty;

        [Required(ErrorMessage = "Az irányítószám megadása kötelező.")]
        [Range(1000, 9999, ErrorMessage = "Az irányítószámnak 4 jegyű számnak kell lennie.")]
        public int IranyitoSzam { get; set; }

        [Required(ErrorMessage = "A város megadása kötelező.")]
        [StringLength(30, ErrorMessage = "A város neve legfeljebb 30 karakter lehet.")]
        public string Varos { get; set; } = String.Empty;

        [Required(ErrorMessage = "Az utca megadása kötelező.")]
        [StringLength(50, ErrorMessage = "Az utca neve legfeljebb 50 karakter lehet.")]
        public string Utca { get; set; } = String.Empty;

        [Required(ErrorMessage = "A házszám megadása kötelező.")]
        [StringLength(10, ErrorMessage = "A házszám legfeljebb 10 karakter lehet.")]
        public string Hazszam { get; set; } = String.Empty;


        // 1 vendég -> több foglalás
        public ICollection<Foglalas> Foglalasok { get; set; } = new List<Foglalas>();
    }
}
