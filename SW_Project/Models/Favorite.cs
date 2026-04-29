using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SW_Project.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string UserId { get; set; } 

        [Required]
        public int ListingId { get; set; } 

        [ForeignKey("ListingId")]
        public Listing Listing { get; set; } 

        public DateTime SavedAt { get; set; } = DateTime.Now; 
    }
}