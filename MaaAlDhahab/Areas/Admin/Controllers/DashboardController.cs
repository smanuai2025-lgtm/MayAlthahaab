using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaaAlDhahab.Data;
using MaaAlDhahab.Models;

namespace MaaAlDhahab.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var vm = new AdminDashboardViewModel
            {
                TotalOrders = await _context.Orders.CountAsync(),
                TodayOrders = await _context.Orders.CountAsync(o => o.CreatedAt >= today),
                TotalRevenue = await _context.Orders
                    .Where(o => o.PaymentStatus == "Paid")
                    .SumAsync(o => o.Total),
                MonthlyRevenue = await _context.Orders
                    .Where(o => o.CreatedAt >= monthStart && o.PaymentStatus == "Paid")
                    .SumAsync(o => o.Total),
                TotalProducts = await _context.Products.CountAsync(p => p.IsActive),
                TotalCustomers = await _context.Users.CountAsync(),
                PendingOrders = await _context.Orders.CountAsync(o => o.OrderStatus == "Pending"),
                RecentOrders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(10)
                    .ToListAsync(),
                TopProducts = await _context.Products
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.ReviewCount)
                    .Take(5)
                    .ToListAsync()
            };

            return View(vm);
        }
    }
}
