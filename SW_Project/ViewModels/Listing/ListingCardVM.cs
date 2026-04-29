// ViewModels/Listing/ListingCardVM.cs
namespace SW_Project.ViewModels.Listing
{
    public class ListingCardVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public decimal PricePerDay { get; set; }
        public string Status { get; set; }
        public string MainImageUrl { get; set; }
        public string CategoryIcon { get; set; }
        public string CategoryName { get; set; }
        public decimal? Deposit { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}