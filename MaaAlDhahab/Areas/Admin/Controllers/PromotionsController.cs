using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaaAlDhahab.Data;
using MaaAlDhahab.Models;

namespace MaaAlDhahab.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PromotionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PromotionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Promotions.OrderByDescending(p => p.CreatedAt).ToListAsync());
        }

        public IActionResult Create() => View(new Promotion());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Promotion promo)
        {
            if (ModelState.IsValid)
            {
                promo.Code = promo.Code.ToUpper();
                promo.CreatedAt = DateTime.UtcNow;
                _context.Promotions.Add(promo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم إنشاء الكود بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            return View(promo);
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            var promo = await _context.Promotions.FindAsync(id);
            if (promo == null) return Json(new { success = false });
            promo.IsActive = !promo.IsActive;
            await _context.SaveChangesAsync();
            return Json(new { success = true, isActive = promo.IsActive });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var promo = await _context.Promotions.FindAsync(id);
            if (promo != null)
            {
                _context.Promotions.Remove(promo);
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }

        // Banners management
        public async Task<IActionResult> Banners()
        {
            return View(await _context.Banners.OrderBy(b => b.SortOrder).ToListAsync());
        }

        public IActionResult CreateBanner() => View(new Banner());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBanner(Banner banner)
        {
            if (ModelState.IsValid)
            {
                _context.Banners.Add(banner);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم إنشاء البانر بنجاح!";
                return RedirectToAction(nameof(Banners));
            }
            return View(banner);
        }
    }
}
