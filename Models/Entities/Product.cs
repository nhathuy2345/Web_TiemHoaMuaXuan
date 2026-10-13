using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? OldPrice { get; set; }

        public int DiscountPercent { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }

        public int Stock { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsFeatured { get; set; }

        // Navigation — Thuộc về 1 Category
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}