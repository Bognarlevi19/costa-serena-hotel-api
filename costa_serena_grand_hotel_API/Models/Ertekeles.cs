using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace costa_serena_grand_hotel_API.Models
{
    [Table("ertekeles")]
    public class Ertekeles
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        public string Nev { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(600)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? IdentityUserId { get; set; }

        [ForeignKey(nameof(IdentityUserId))]
        public IdentityUser? IdentityUser { get; set; }
    }
}
