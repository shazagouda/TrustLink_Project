using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SW_Project.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string UserId { get; set; } 

        [Required]
        [StringLength(300)]
        public string Message { get; set; } 

        [Required]
        [StringLength(50)]
        public string Type { get; set; } 

        public bool IsRead { get; set; } = false;

        public string? LinkUrl { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}