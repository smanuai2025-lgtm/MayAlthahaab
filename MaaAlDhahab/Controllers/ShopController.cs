using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaaAlDhahab.Data;
using MaaAlDhahab.Models;

namespace MaaAlDhahab.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(ShopFilterViewModel filter)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(p => p.NameAr.Contains(filter.Search) || p.NameEn.Contains(filter.Search));

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (!string.IsNullOrEmpty(filter.Gender))
                query = query.Where(p => p.Gender == filter.Gender);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.PriceKWD >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.PriceKWD <= filter.MaxPrice.Value);

            // Sorting
            query = filter.SortBy switch
            {
                "price_asc" => query.OrderBy(p => p.PriceKWD),
                "price_desc" => query.OrderByDescending(p => p.PriceKWD),
                "rating" => query.OrderByDescending(p => p.Rating),
                "bestseller" => query.OrderByDescending(p => p.IsBestSeller),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var products = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var viewModel = new ShopViewModel
            {
                Products = products,
                Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.SortOrder).ToListAsync(),
                Filter = filter,
                TotalCount = totalCount
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_ProductGrid", viewModel);

            return View(viewModel);
        }

        // AJAX endpoint for load more
        [HttpGet]
        public async Task<IActionResult> LoadMore(ShopFilterViewModel filter)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (!string.IsNullOrEmpty(filter.Gender))
                query = query.Where(p => p.Gender == filter.Gender);

            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new
                {
                    p.Id,
                    p.NameAr,
                    p.NameEn,
                    p.PriceKWD,
                    p.OriginalPriceKWD,
                    p.ImageUrl,
                    p.Rating,
                    p.ReviewCount,
                    p.IsBestSeller,
                    p.IsNewArrival,
                    CategoryName = p.Category!.NameAr
                })
                .ToListAsync();

            return Json(products);
        }
    }
}
