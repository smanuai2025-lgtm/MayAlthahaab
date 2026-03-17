using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaaAlDhahab.Data;
using MaaAlDhahab.Models;

namespace MaaAlDhahab.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoriesController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Products.Where(p => p.IsActive))
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
            return View(categories);
        }
    }
}
