using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using MaaAlDhahab.Data;
using MaaAlDhahab.Models;

namespace MaaAlDhahab.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null) return NotFound();

            var relatedProducts = await _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id && p.IsActive)
                .Take(4)
                .ToListAsync();

            ScentNotes? scentNotes = null;
            List<ProductSize>? sizes = null;

            try
            {
                scentNotes = JsonSerializer.Deserialize<ScentNotes>(product.ScentNotesJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch { }

            try
            {
                sizes = JsonSerializer.Deserialize<List<ProductSize>>(product.SizesJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch { }

            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts,
                Reviews = product.Reviews.Where(r => r.IsApproved).OrderByDescending(r => r.CreatedAt).ToList(),
                ScentNotes = scentNotes,
                Sizes = sizes
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] Review review)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false });

            review.IsApproved = true;
            review.CreatedAt = DateTime.UtcNow;

            if (User.Identity?.IsAuthenticated == true)
                review.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            _context.Reviews.Add(review);

            // Update product rating
            var product = await _context.Products.FindAsync(review.ProductId);
            if (product != null)
            {
                var allReviews = await _context.Reviews
                    .Where(r => r.ProductId == review.ProductId && r.IsApproved)
                    .ToListAsync();
                product.Rating = allReviews.Any()
                    ? allReviews.Average(r => r.Rating)
                    : review.Rating;
                product.ReviewCount = allReviews.Count + 1;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "تم إضافة تقييمك بنجاح! شكراً لك 🌟" });
        }

        // Quick search API
        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrEmpty(q) || q.Length < 2)
                return Json(new List<object>());

            var results = await _context.Products
                .Where(p => p.IsActive && (p.NameAr.Contains(q) || p.NameEn.Contains(q)))
                .Take(5)
                .Select(p => new { p.Id, p.NameAr, p.NameEn, p.PriceKWD, p.ImageUrl })
                .ToListAsync();

            return Json(results);
        }
    }
}
