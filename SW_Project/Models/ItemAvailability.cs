using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SW_Project.Models
{
    public class ItemAvailability
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; } 

        [ForeignKey("ItemId")]
        public Listing Listing { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}