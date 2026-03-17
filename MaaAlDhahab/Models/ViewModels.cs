using System.ComponentModel.DataAnnotations;

namespace MaaAlDhahab.Models
{
    // Cart ViewModels
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductNameEn { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new();
        public decimal SubTotal => Items.Sum(i => i.UnitPrice * i.Quantity);
        public decimal DeliveryFee { get; set; } = 1.500m;
        public decimal Discount { get; set; }
        public decimal Total => SubTotal + DeliveryFee - Discount;
        public string? CouponCode { get; set; }
    }

    // Checkout ViewModels
    public class CheckoutViewModel
    {
        [Required] public string CustomerName { get; set; } = string.Empty;
        [Required, EmailAddress] public string CustomerEmail { get; set; } = string.Empty;
        [Required] public string CustomerPhone { get; set; } = string.Empty;
        [Required] public string Governorate { get; set; } = string.Empty;
        [Required] public string Area { get; set; } = string.Empty;
        [Required] public string Block { get; set; } = string.Empty;
        [Required] public string Street { get; set; } = string.Empty;
        [Required] public string HouseNumber { get; set; } = string.Empty;
        public string? AddressNotes { get; set; }
        public string PaymentMethod { get; set; } = "KNET";
        public string? CouponCode { get; set; }
        public CartViewModel? Cart { get; set; }
    }

    // Account ViewModels
    public class LoginViewModel
    {
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required] public string FullNameAr { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, Phone] public string PhoneNumber { get; set; } = string.Empty;
        [Required, DataType(DataType.Password), MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required, DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? ReferralCode { get; set; }
    }

    // Shop/Filter ViewModels
    public class ShopFilterViewModel
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public string? Gender { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } = "newest";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    public class ShopViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public ShopFilterViewModel Filter { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / Filter.PageSize);
    }

    // Home ViewModels
    public class HomeViewModel
    {
        public List<Banner> Banners { get; set; } = new();
        public List<Product> FeaturedProducts { get; set; } = new();
        public List<Product> NewArrivals { get; set; } = new();
        public List<Product> BestSellers { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Review> Testimonials { get; set; } = new();
    }

    // Product Detail ViewModel
    public class ProductDetailViewModel
    {
        public Product? Product { get; set; }
        public List<Product> RelatedProducts { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public ScentNotes? ScentNotes { get; set; }
        public List<ProductSize>? Sizes { get; set; }
    }

    public class ScentNotes
    {
        public List<string> Top { get; set; } = new();
        public List<string> Middle { get; set; } = new();
        public List<string> Base { get; set; } = new();
    }

    public class ProductSize
    {
        public string Size { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    // Admin ViewModels
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int TodayOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }
        public int PendingOrders { get; set; }
        public List<Order> RecentOrders { get; set; } = new();
        public List<Product> TopProducts { get; set; } = new();
    }
}
