using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SW_Project.Data;
using SW_Project.Models;
using System.Threading.Tasks;

namespace SW_Project.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(ContactMessage model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            if (!model.AgreePrivacy)
            {
                ModelState.AddModelError("AgreePrivacy", "You must agree to the privacy policy.");
                return View("Index", model);
            }

            model.SentAt = DateTime.Now;
            model.IsRead = false;

            _context.ContactMessages.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your message has been sent successfully. We'll get back to you soon.";
            return RedirectToAction("Index");
        }
    }
}