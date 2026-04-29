using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SW_Project.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string ReporterId { get; set; } 

        public string? TargetUserId { get; set; } 

        public int? TargetListingId { get; set; } 

        [Required]
        public string Reason { get; set; } 

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Open"; 

        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}