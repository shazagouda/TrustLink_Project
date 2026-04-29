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
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoritesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // عرض صفحة المفضلة
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var favorites = await _context.Favorites
                .Include(f => f.Listing)
                .ThenInclude(l => l.Category)
                .Include(f => f.Listing.ListingImages)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.SavedAt)
                .ToListAsync();
            return View(favorites);
        }

        // إضافة إلى المفضلة (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int listingId)
        {
            var userId = _userManager.GetUserId(User);
            var existing = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ListingId == listingId);
            if (existing != null)
                return Json(new { success = false, message = "Already in favorites" });

            var favorite = new Favorite
            {
                UserId = userId,
                ListingId = listingId,
                SavedAt = DateTime.Now
            };
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Added to favorites" });
        }

        // إزالة من المفضلة (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int listingId)
        {
            var userId = _userManager.GetUserId(User);
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ListingId == listingId);
            if (favorite == null)
                return Json(new { success = false, message = "Not in favorites" });

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Removed from favorites" });
        }
    }
}