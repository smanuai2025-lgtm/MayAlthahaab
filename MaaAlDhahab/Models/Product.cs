using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaaAlDhahab.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string NameAr { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string NameEn { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string DescriptionAr { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string DescriptionEn { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,3)")]
        public decimal PriceKWD { get; set; }

        [Column(TypeName = "decimal(10,3)")]
        public decimal? OriginalPriceKWD { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string ImageUrl { get; set; } = "/images/product-placeholder.jpg";
        public string? ImageUrl2 { get; set; }
        public string? ImageUrl3 { get; set; }

        // JSON stored as string: {"top": ["Bergamot", "Lemon"], "middle": ["Rose", "Jasmine"], "base": ["Musk", "Amber"]}
        public string ScentNotesJson { get; set; } = "{}";

        // JSON stored as string: [{"size": "65ml", "price": 5.000}, {"size": "75ml", "price": 6.500}]
        public string SizesJson { get; set; } = "[]";

        public string? Gender { get; set; } // Male, Female, Unisex, Kids

        public bool IsFeatured { get; set; }
        public bool IsNewArrival { get; set; }
        public bool IsBestSeller { get; set; }
        public bool IsActive { get; set; } = true;

        public int StockQuantity { get; set; } = 100;
        public double Rating { get; set; } = 5.0;
        public int ReviewCount { get; set; }

        public string? Concentration { get; set; } // EDP, EDT, Parfum, Bukhoor

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
