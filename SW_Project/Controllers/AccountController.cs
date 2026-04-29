using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;  
using SW_Project.Data;                
using SW_Project.Models;
using System.Linq;                     
using System.Threading.Tasks;

namespace SW_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;   

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)                 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;                            
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                TempData["Success"] = $"Welcome back, {user.Name}!";
                return RedirectToAction("Profile", "Account");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!model.AgreeToTerms)
            {
                ModelState.AddModelError("AgreeToTerms", "You must agree to the terms.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                Location = model.Location,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["Success"] = $"Welcome to TrustLink, {user.Name}!";
                return RedirectToAction("Profile", "Account");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    System.Diagnostics.Debug.WriteLine($"ERROR: {error.Description}");
                }
                return View(model);
            }
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var userListings = await _context.Listings
                .Include(l => l.Category)
                .Include(l => l.ListingImages)
                .Where(l => l.OwnerId == user.Id)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            user.Listings = userListings;
            return View(user);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string Name, string PhoneNumber, string Location)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            bool changed = false;
            if (!string.IsNullOrEmpty(Name) && user.Name != Name) { user.Name = Name; changed = true; }
            if (PhoneNumber != null && user.PhoneNumber != PhoneNumber) { user.PhoneNumber = PhoneNumber; changed = true; }
            if (Location != null && user.Location != Location) { user.Location = Location; changed = true; }

            if (changed)
            {
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    TempData["Success"] = "Profile updated!";
                }
                else TempData["Error"] = "Update failed.";
            }
            else TempData["Info"] = "No changes.";
            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "Logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}