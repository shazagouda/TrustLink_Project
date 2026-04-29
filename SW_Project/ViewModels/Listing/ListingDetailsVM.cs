// ViewModels/Listing/ListingDetailsVM.cs
namespace SW_Project.ViewModels.Listing
{
    public class ListingDetailsVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal? Deposit { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Category info
        public string CategoryName { get; set; }
        public string CategoryIcon { get; set; }

        // Owner info
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerLocation { get; set; }
        public double OwnerRating { get; set; }
        public DateTime OwnerJoinedAt { get; set; }
        public string OwnerAvatar { get; set; }

        // Images
        public List<string> ImageUrls { get; set; }
        public string MainImageUrl { get; set; }

        // UI flags
        public bool IsOwner { get; set; }
        public bool IsAvailable { get; set; }
    }
}