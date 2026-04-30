// ViewModels/Message/InboxVM.cs
namespace SW_Project.ViewModels.Message
{
    public class InboxVM
    {
        public List<InboxItemVM> Conversations { get; set; }
        public int UnreadCount { get; set; }
    }

    public class InboxItemVM
    {
        public int ConversationId { get; set; }
        public string OtherUserId { get; set; }
        public string OtherUserName { get; set; }
        public string OtherUserAvatar { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
        public int? ListingId { get; set; }
        public string ListingTitle { get; set; }
        public string ListingImageUrl { get; set; }
    }
}