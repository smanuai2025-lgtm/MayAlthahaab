using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MaaAlDhahab.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)] public string FullNameAr { get; set; } = string.Empty;
        [MaxLength(100)] public string FullNameEn { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PreferredLanguage { get; set; } = "ar";
        public int LoyaltyPoints { get; set; }
        public string? ReferralCode { get; set; }
        public string? ReferredByCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
