using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace costa_serena_grand_hotel_API.Models
{
    [Table("szoba_kep")]
    public class SzobaKep
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string KepUrl { get; set; } = string.Empty;

        public bool FoKep { get; set; } = false;

        public int Sorrend { get; set; } = 0;

        [ForeignKey(nameof(Szoba))]
        public int SzobaId { get; set; }

        public Szoba? Szoba { get; set; }
    }
}