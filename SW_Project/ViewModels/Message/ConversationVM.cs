// ViewModels/Message/ConversationVM.cs
namespace SW_Project.ViewModels.Message
{
    public class ConversationVM
    {
        public int ConversationId { get; set; }
        public string OtherUserId { get; set; }
        public string OtherUserName { get; set; }
        public string OtherUserAvatar { get; set; }
        public int? ListingId { get; set; }
        public string ListingTitle { get; set; }
        public string ListingImageUrl { get; set; }
        public List<MessageItemVM> Messages { get; set; }
        public SendMessageVM SendMessage { get; set; }
    }

    public class MessageItemVM
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public bool IsFromCurrentUser { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}