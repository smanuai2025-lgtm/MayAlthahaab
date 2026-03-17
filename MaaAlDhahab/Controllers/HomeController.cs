using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaaAlDhahab.Data;
using MaaAlDhahab.Models;

namespace MaaAlDhahab.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                Banners = await _context.Banners
                    .Where(b => b.IsActive && b.Position == "Hero")
                    .OrderBy(b => b.SortOrder)
                    .ToListAsync(),
                FeaturedProducts = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsActive && p.IsFeatured)
                    .Take(6)
                    .ToListAsync(),
                NewArrivals = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsActive && p.IsNewArrival)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(4)
                    .ToListAsync(),
                BestSellers = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsActive && p.IsBestSeller)
                    .Take(4)
                    .ToListAsync(),
                Categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.SortOrder)
                    .ToListAsync(),
                Testimonials = await _context.Reviews
                    .Where(r => r.IsApproved)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(6)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe([FromBody] NewsletterSubscriber subscriber)
        {
            if (string.IsNullOrEmpty(subscriber.Email))
                return Json(new { success = false, message = "البريد الإلكتروني مطلوب" });

            var exists = await _context.NewsletterSubscribers
                .AnyAsync(n => n.Email == subscriber.Email);

            if (exists)
                return Json(new { success = false, message = "أنت مشترك بالفعل!" });

            subscriber.IsActive = true;
            subscriber.SubscribedAt = DateTime.UtcNow;
            _context.NewsletterSubscribers.Add(subscriber);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "شكراً! تم اشتراكك بنجاح 🎉" });
        }

        public IActionResult Privacy() => View();
        public IActionResult Terms() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
