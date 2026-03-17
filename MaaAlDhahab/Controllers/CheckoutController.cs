using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using MaaAlDhahab.Data;
using MaaAlDhahab.Models;

namespace MaaAlDhahab.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "Cart";

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        private CartViewModel GetCart()
        {
            var json = HttpContext.Session.GetString(CartSessionKey);
            return json != null
                ? JsonSerializer.Deserialize<CartViewModel>(json) ?? new CartViewModel()
                : new CartViewModel();
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            if (!cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            var vm = new CheckoutViewModel { Cart = cart };

            // Pre-fill if logged in
            if (User.Identity?.IsAuthenticated == true)
            {
                vm.CustomerEmail = User.Identity.Name ?? "";
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var cart = GetCart();
            if (!cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            model.Cart = cart;

            if (!ModelState.IsValid)
                return View("Index", model);

            var order = new Order
            {
                OrderNumber = $"MAD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                Governorate = model.Governorate,
                Area = model.Area,
                Block = model.Block,
                Street = model.Street,
                HouseNumber = model.HouseNumber,
                AddressNotes = model.AddressNotes,
                PaymentMethod = model.PaymentMethod,
                SubTotal = cart.SubTotal,
                DeliveryFee = cart.DeliveryFee,
                Discount = cart.Discount,
                Total = cart.Total,
                CouponCode = cart.CouponCode,
                OrderStatus = "Pending",
                PaymentStatus = model.PaymentMethod == "Cash" ? "Pending" : "Pending",
                CreatedAt = DateTime.UtcNow
            };

            // Attach user if logged in
            if (User.Identity?.IsAuthenticated == true)
            {
                order.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            }

            foreach (var item in cart.Items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Size = item.Size,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Total = item.UnitPrice * item.Quantity
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Clear cart
            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToAction("Confirmation", new { orderNumber = order.OrderNumber });
        }

        public async Task<IActionResult> Confirmation(string orderNumber)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null) return NotFound();
            return View(order);
        }

        // AJAX: Validate coupon during checkout
        [HttpPost]
        public async Task<IActionResult> ValidateCoupon([FromBody] CouponRequest request)
        {
            var promo = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code == request.Code.ToUpper() && p.IsActive);

            if (promo == null)
                return Json(new { valid = false, message = "كود غير صحيح" });

            return Json(new
            {
                valid = true,
                discountType = promo.DiscountType,
                discountValue = promo.DiscountValue,
                description = promo.DescriptionAr
            });
        }
    }
}
