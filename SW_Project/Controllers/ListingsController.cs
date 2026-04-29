using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SW_Project.Data;
using SW_Project.Models;
using System.IO;
using System.Linq;

namespace SW_Project.Controllers
{
    [Authorize]
    public class ListingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ListingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(
            int page = 1,
            string search = "",
            int? category = null,
            string location = "",
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sort = "newest")
        {
            int pageSize = 9;
            var query = _context.Listings
                .Include(l => l.Category)
                .Include(l => l.ListingImages)
                .Where(l => l.Status == "Available");

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(l => l.Title.Contains(search) || l.Description.Contains(search) || l.Location.Contains(search));
            if (category.HasValue)
                query = query.Where(l => l.CategoryId == category.Value);
            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(l => l.Location.Contains(location));
            if (minPrice.HasValue)
                query = query.Where(l => l.PricePerDay >= minPrice);
            if (maxPrice.HasValue)
                query = query.Where(l => l.PricePerDay <= maxPrice);

            sort = sort?.ToLower() ?? "newest";
            query = sort switch
            {
                "price-asc" => query.OrderBy(l => l.PricePerDay),
                "price-desc" => query.OrderByDescending(l => l.PricePerDay),
                _ => query.OrderByDescending(l => l.CreatedAt)
            };

            int total = await query.CountAsync();
            var listings = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.TotalListings = total;
            ViewBag.Search = search;
            ViewBag.SelectedCategory = category;
            ViewBag.Location = location;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Sort = sort;
            ViewBag.Categories = categories;

            return View(listings);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var listing = await _context.Listings
                .Include(l => l.Category)
                .Include(l => l.Owner)
                .Include(l => l.ListingImages)
                .FirstOrDefaultAsync(l => l.Id == id);
            if (listing == null)
                return NotFound();
            return View(listing);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Listing listing, List<IFormFile> images)
        {
            ModelState.Remove("Category");
            ModelState.Remove("Owner");
            ModelState.Remove("ListingImages");
            ModelState.Remove("OwnerId");

            System.Diagnostics.Debug.WriteLine("===== POST CREATE HIT =====");
            System.Diagnostics.Debug.WriteLine($"Title: {listing?.Title}");
            System.Diagnostics.Debug.WriteLine($"CategoryId received: {listing?.CategoryId}");
            System.Diagnostics.Debug.WriteLine($"ModelState.IsValid after removal: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                System.Diagnostics.Debug.WriteLine("ModelState Errors: " + string.Join("; ", errors));
            }

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                listing.OwnerId = userId;
                listing.Status = "Available";
                listing.CreatedAt = DateTime.Now;

                _context.Listings.Add(listing);
                await _context.SaveChangesAsync();

                if (images != null && images.Any())
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "listings");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    bool isFirst = true;
                    foreach (var img in images)
                    {
                        if (img.Length > 0)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(img.FileName);
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await img.CopyToAsync(stream);
                            }

                            var listingImage = new ListingImage
                            {
                                ImagePath = "/uploads/listings/" + uniqueFileName,
                                IsMain = isFirst,
                                ListingId = listing.Id
                            };
                            _context.ListingImages.Add(listingImage);
                            isFirst = false;
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Your listing has been posted!";
                return RedirectToAction("Details", new { id = listing.Id });
            }

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            return View(listing);
        }

        [Authorize]
        public async Task<IActionResult> MyListings()
        {
            var userId = _userManager.GetUserId(User);
            var myListings = await _context.Listings
                .Include(l => l.Category)
                .Include(l => l.ListingImages)
                .Where(l => l.OwnerId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
            return View(myListings);
        }
    }
}