using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace SW_Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        public string? ProfileImage { get; set; }

        public string? Location { get; set; }

        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Rating { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}