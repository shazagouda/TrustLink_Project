using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SW_Project.Models
{
    public class ContractSignature
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public int ContractId { get; set; } 

        [ForeignKey("ContractId")]
        public Contract Contract { get; set; } 

        [Required]
        public string UserId { get; set; }

        [Required]
        public string SignatureImage { get; set; } 

        public DateTime SignedAt { get; set; } = DateTime.Now; 
    }
}