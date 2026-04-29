using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SW_Project.Data;
using SW_Project.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SW_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;


        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var recentListings = await _context.Listings
                .Include(l => l.Category)
                .Include(l => l.ListingImages)
                .Where(l => l.Status == "Available")
                .OrderByDescending(l => l.CreatedAt)
                .Take(3)
                .ToListAsync();

            return View(recentListings);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult HowItWorks()
        {
            ViewData["Title"] = "How It Works - TrustLink";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Title"] = "Contact Us - TrustLink";
            return View();
        }

        
    }
}