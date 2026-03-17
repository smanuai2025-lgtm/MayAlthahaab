using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaaAlDhahab.Data;
using MaaAlDhahab.Models;

namespace MaaAlDhahab.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? status)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.OrderStatus == status);

            ViewBag.StatusFilter = status;
            return View(await query.OrderByDescending(o => o.CreatedAt).ToListAsync());
        }

        public async Task<IActionResult> Detail(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusRequest request)
        {
            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null) return Json(new { success = false });

            order.OrderStatus = request.Status;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }

    public class UpdateOrderStatusRequest
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
