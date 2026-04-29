using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SW_Project.Data;
using SW_Project.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SW_Project.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyBookings()
        {
            var userId = _userManager.GetUserId(User);
            var bookings = await _context.Bookings
                .Include(b => b.Listing)
                .ThenInclude(l => l.Category)
                .Include(b => b.Listing.ListingImages)
                .Where(b => b.RenterId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            return View(bookings);
        }

        public async Task<IActionResult> ReceivedBookings()
        {
            var userId = _userManager.GetUserId(User);
            var bookings = await _context.Bookings
                .Include(b => b.Listing)
                .ThenInclude(l => l.Category)
                .Include(b => b.Renter)
                .Where(b => b.Listing.OwnerId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            return View(bookings);
        }

        public async Task<IActionResult> Create(int listingId)
        {
            var listing = await _context.Listings
                .Include(l => l.Category)
                .FirstOrDefaultAsync(l => l.Id == listingId);
            if (listing == null)
                return NotFound();

            ViewBag.Listing = listing;
            return View(new Booking { ListingId = listingId, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            ModelState.Remove("Listing");
            ModelState.Remove("Renter");
            ModelState.Remove("TotalPrice");
            ModelState.Remove("RenterId");   

            var listing = await _context.Listings.FindAsync(booking.ListingId);
            if (listing == null)
                return NotFound();

            if (booking.StartDate < DateTime.Today)
            {
                ModelState.AddModelError("StartDate", "Start date cannot be in the past.");
            }
            if (booking.EndDate <= booking.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
            }

            
            var conflictingBooking = await _context.Bookings
                .Where(b => b.ListingId == booking.ListingId &&
                            b.Status != "Rejected" &&
                            b.Status != "Cancelled" &&
                            b.Status != "Completed" &&
                            ((booking.StartDate >= b.StartDate && booking.StartDate < b.EndDate) ||
                             (booking.EndDate > b.StartDate && booking.EndDate <= b.EndDate) ||
                             (booking.StartDate <= b.StartDate && booking.EndDate >= b.EndDate)))
                .FirstOrDefaultAsync();

            if (conflictingBooking != null)
            {
                ModelState.AddModelError(string.Empty, "This item is already booked for the selected dates. Please choose different dates.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Listing = listing;
                return View(booking);
            }

            int days = (booking.EndDate - booking.StartDate).Days;
            booking.TotalPrice = listing.PricePerDay * days;
            booking.DepositPaid = listing.Deposit;
            booking.RenterId = _userManager.GetUserId(User);
            booking.Status = "Pending";
            booking.CreatedAt = DateTime.Now;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your booking request has been sent. The owner will review it.";
            return RedirectToAction("MyBookings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var booking = await _context.Bookings
                .Include(b => b.Listing)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            if (booking.Listing.OwnerId != userId)
                return Forbid();

            if (status != "Accepted" && status != "Rejected")
                return BadRequest();

            booking.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Booking has been {status.ToLower()}.";
            return RedirectToAction("ReceivedBookings");
        }
    }
}