using System.ComponentModel.DataAnnotations;

namespace MaaAlDhahab.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string NameAr { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string NameEn { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? IconClass { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
