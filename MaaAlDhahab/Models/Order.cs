using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaaAlDhahab.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Guest info
        [MaxLength(100)] public string CustomerName { get; set; } = string.Empty;
        [MaxLength(100)] public string CustomerEmail { get; set; } = string.Empty;
        [MaxLength(20)] public string CustomerPhone { get; set; } = string.Empty;

        // Kuwait address
        [MaxLength(100)] public string Governorate { get; set; } = string.Empty;
        [MaxLength(100)] public string Area { get; set; } = string.Empty;
        [MaxLength(200)] public string Block { get; set; } = string.Empty;
        [MaxLength(50)] public string Street { get; set; } = string.Empty;
        [MaxLength(50)] public string HouseNumber { get; set; } = string.Empty;
        [MaxLength(500)] public string? AddressNotes { get; set; }

        [Column(TypeName = "decimal(10,3)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(10,3)")]
        public decimal DeliveryFee { get; set; } = 1.500m;

        [Column(TypeName = "decimal(10,3)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(10,3)")]
        public decimal Total { get; set; }

        public string? CouponCode { get; set; }
        public string PaymentMethod { get; set; } = "KNET"; // KNET, Cash
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed
        public string OrderStatus { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled

        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [MaxLength(200)] public string ProductName { get; set; } = string.Empty;
        [MaxLength(20)] public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,3)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(10,3)")]
        public decimal Total { get; set; }
    }
}
