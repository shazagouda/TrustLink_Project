// ViewModels/Message/SendMessageVM.cs
using System.ComponentModel.DataAnnotations;

namespace SW_Project.ViewModels.Message
{
    public class SendMessageVM
    {
        [Required(ErrorMessage = "Message cannot be empty")]
        [MinLength(1, ErrorMessage = "Message cannot be empty")]
        [MaxLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
        public string Text { get; set; }

        public int? ListingId { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string ListingTitle { get; set; }
    }
}