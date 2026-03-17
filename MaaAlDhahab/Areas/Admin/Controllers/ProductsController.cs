using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaaAlDhahab.Data;
using MaaAlDhahab.Models;

namespace MaaAlDhahab.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.NameAr.Contains(search) || p.NameEn.Contains(search));
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(await query.OrderByDescending(p => p.CreatedAt).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "NameAr");
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم إضافة المنتج بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "NameAr");
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "NameAr", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();
            if (ModelState.IsValid)
            {
                product.UpdatedAt = DateTime.UtcNow;
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم تحديث المنتج بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "NameAr", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsActive = false;
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFeatured(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return Json(new { success = false });
            product.IsFeatured = !product.IsFeatured;
            await _context.SaveChangesAsync();
            return Json(new { success = true, isFeatured = product.IsFeatured });
        }
    }
}
