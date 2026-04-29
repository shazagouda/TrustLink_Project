namespace SW_Project.ViewModels.Account
{
    public class ProfileVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProfileImage { get; set; }

        
        public int ActiveListingsCount { get; set; }
        public int TotalBookingsCount { get; set; }
        public int ActiveContractsCount { get; set; }
        public int FavoritesCount { get; set; }

        
        public List<ListingCardVM> MyListings { get; set; }
        public List<BookingCardVM> MyBookings { get; set; }
        public List<ContractCardVM> MyContracts { get; set; }
    }

   
    public class ListingCardVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public decimal PricePerDay { get; set; }
        public string Status { get; set; }
        public string MainImageUrl { get; set; }
        public string CategoryIcon { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    
    public class BookingCardVM
    {
        public int Id { get; set; }
        public int ListingId { get; set; }
        public string ListingTitle { get; set; }
        public string ListingImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public bool IsOwner { get; set; } 
        public string OtherPartyName { get; set; }
    }

    
    public class ContractCardVM
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; }
        public string ListingTitle { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OtherPartyName { get; set; }
        public bool IsSigned { get; set; }
    }
}
