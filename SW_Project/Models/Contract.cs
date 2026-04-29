using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SW_Project.Models
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }  

        [Required]
        public int BookingId { get; set; } 

        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }

        [Required]
        public string PartyAId { get; set; } 

        [Required]
        public string PartyBId { get; set; } 

        [Required]
        [StringLength(200)]
        public string Title { get; set; }  

        [Required]
        public string Terms { get; set; } 

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Draft"; 

        public string? PdfPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}