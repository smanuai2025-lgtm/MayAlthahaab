using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using MaaAlDhahab.Data;
using MaaAlDhahab.Models;

namespace MaaAlDhahab.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "Cart";

        public CartController(ApplicationDbContext context)
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

        private void SaveCart(CartViewModel cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddToCartRequest request)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
                return Json(new { success = false, message = "المنتج غير موجود" });

            var cart = GetCart();
            var existingItem = cart.Items.FirstOrDefault(i =>
                i.ProductId == request.ProductId && i.Size == request.Size);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = request.ProductId,
                    ProductName = product.NameAr,
                    ProductNameEn = product.NameEn,
                    Size = request.Size,
                    Quantity = request.Quantity,
                    UnitPrice = request.Price > 0 ? request.Price : product.PriceKWD,
                    ImageUrl = product.ImageUrl
                });
            }

            SaveCart(cart);
            return Json(new
            {
                success = true,
                message = "تمت الإضافة للسلة! 🛒",
                cartCount = cart.Items.Sum(i => i.Quantity),
                cartTotal = cart.SubTotal.ToString("F3")
            });
        }

        [HttpPost]
        public IActionResult UpdateQuantity([FromBody] UpdateCartRequest request)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i =>
                i.ProductId == request.ProductId && i.Size == request.Size);

            if (item == null)
                return Json(new { success = false });

            if (request.Quantity <= 0)
                cart.Items.Remove(item);
            else
                item.Quantity = request.Quantity;

            SaveCart(cart);
            return Json(new
            {
                success = true,
                itemTotal = (item.UnitPrice * request.Quantity).ToString("F3"),
                cartSubTotal = cart.SubTotal.ToString("F3"),
                cartTotal = cart.Total.ToString("F3"),
                cartCount = cart.Items.Sum(i => i.Quantity)
            });
        }

        [HttpPost]
        public IActionResult Remove([FromBody] RemoveCartRequest request)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i =>
                i.ProductId == request.ProductId && i.Size == request.Size);

            if (item != null) cart.Items.Remove(item);

            SaveCart(cart);
            return Json(new
            {
                success = true,
                cartCount = cart.Items.Sum(i => i.Quantity),
                cartSubTotal = cart.SubTotal.ToString("F3"),
                cartTotal = cart.Total.ToString("F3")
            });
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon([FromBody] CouponRequest request)
        {
            var promo = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code == request.Code.ToUpper() && p.IsActive);

            if (promo == null)
                return Json(new { success = false, message = "كود الخصم غير صحيح أو منتهي الصلاحية" });

            if (promo.EndDate.HasValue && promo.EndDate.Value < DateTime.UtcNow)
                return Json(new { success = false, message = "انتهت صلاحية كود الخصم" });

            var cart = GetCart();

            if (promo.MinOrderAmount.HasValue && cart.SubTotal < promo.MinOrderAmount.Value)
                return Json(new
                {
                    success = false,
                    message = $"الحد الأدنى للطلب {promo.MinOrderAmount:F3} د.ك"
                });

            decimal discount = promo.DiscountType == "Percentage"
                ? cart.SubTotal * (promo.DiscountValue / 100)
                : promo.DiscountValue;

            cart.Discount = discount;
            cart.CouponCode = promo.Code;
            SaveCart(cart);

            return Json(new
            {
                success = true,
                message = $"تم تطبيق خصم {promo.DescriptionAr} 🎉",
                discount = discount.ToString("F3"),
                cartTotal = cart.Total.ToString("F3")
            });
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            return Json(new { count = cart.Items.Sum(i => i.Quantity) });
        }

        // Wishlist (stored in session for simplicity, can be moved to DB for logged-in users)
        [HttpPost]
        public IActionResult ToggleWishlist([FromBody] WishlistRequest request)
        {
            var wishlist = HttpContext.Session.GetString("Wishlist");
            var items = wishlist != null
                ? JsonSerializer.Deserialize<List<int>>(wishlist) ?? new List<int>()
                : new List<int>();

            bool added;
            if (items.Contains(request.ProductId))
            {
                items.Remove(request.ProductId);
                added = false;
            }
            else
            {
                items.Add(request.ProductId);
                added = true;
            }

            HttpContext.Session.SetString("Wishlist", JsonSerializer.Serialize(items));
            return Json(new { success = true, added, count = items.Count });
        }
    }

    // Request models
    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }
    }

    public class UpdateCartRequest
    {
        public int ProductId { get; set; }
        public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class RemoveCartRequest
    {
        public int ProductId { get; set; }
        public string Size { get; set; } = string.Empty;
    }

    public class CouponRequest
    {
        public string Code { get; set; } = string.Empty;
    }

    public class WishlistRequest
    {
        public int ProductId { get; set; }
    }
}
