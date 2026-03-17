using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaaAlDhahab.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        [Required, MaxLength(50)] public string Code { get; set; } = string.Empty;
        [MaxLength(200)] public string DescriptionAr { get; set; } = string.Empty;
        [MaxLength(200)] public string DescriptionEn { get; set; } = string.Empty;
        public string DiscountType { get; set; } = "Percentage"; // Percentage, Fixed
        [Column(TypeName = "decimal(10,3)")] public decimal DiscountValue { get; set; }
        [Column(TypeName = "decimal(10,3)")] public decimal? MinOrderAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Banner
    {
        public int Id { get; set; }
        [MaxLength(200)] public string TitleAr { get; set; } = string.Empty;
        [MaxLength(200)] public string TitleEn { get; set; } = string.Empty;
        [MaxLength(500)] public string? SubtitleAr { get; set; }
        [MaxLength(500)] public string? SubtitleEn { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public string? ButtonTextAr { get; set; }
        public string? ButtonTextEn { get; set; }
        public string Position { get; set; } = "Hero"; // Hero, Promo, Sidebar
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        [MaxLength(100)] public string ReviewerName { get; set; } = string.Empty;
        [MaxLength(1000)] public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; } = 5;
        public bool IsApproved { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class WishlistItem
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }

    public class NewsletterSubscriber
    {
        public int Id { get; set; }
        [MaxLength(200)] public string Email { get; set; } = string.Empty;
        [MaxLength(100)] public string? Name { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
    }
}
