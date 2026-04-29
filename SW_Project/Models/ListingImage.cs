using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SW_Project.Models
{
    public class ListingImage
    {
        [Key]
        public int Id { get; set; }  

        [Required]
        public string ImagePath { get; set; }  

        public bool IsMain { get; set; } = false;


        [Required]
        public int ListingId { get; set; }  

        [ForeignKey("ListingId")]
        public Listing Listing { get; set; } 
    }
}