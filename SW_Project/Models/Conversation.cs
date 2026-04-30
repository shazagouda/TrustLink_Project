// Models/Conversation.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SW_Project.Models
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ParticipantAId { get; set; }
        [ForeignKey("ParticipantAId")]
        public ApplicationUser ParticipantA { get; set; }

        [Required]
        public string ParticipantBId { get; set; }
        [ForeignKey("ParticipantBId")]
        public ApplicationUser ParticipantB { get; set; }

        public int? ListingId { get; set; }
        [ForeignKey("ListingId")]
        public Listing Listing { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastMessageAt { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}