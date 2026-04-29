using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SW_Project.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ListingId { get; set; }
        [ForeignKey("ListingId")]
        public Listing Listing { get; set; }

        [Required]
        public string RenterId { get; set; }
        [ForeignKey("RenterId")]
        public ApplicationUser Renter { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? DepositPaid { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}