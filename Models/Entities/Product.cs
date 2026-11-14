using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        // ⭐ Đã sửa: Đổi từ 200 thành 100 ký tự và thêm câu thông báo lỗi
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự.")]
        public string Name { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? OldPrice { get; set; }

        // ⭐ Đã thêm: Ràng buộc giảm giá từ 0% đến 50%
        [Range(0, 50, ErrorMessage = "Phần trăm giảm giá phải từ 0% đến 50%.")]
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
        [NotMapped]// thuộc tính này không được ánh xạ vào cơ sở dữ liệu, chỉ dùng để hiển thị thông tin
        public double AverageRating { get; set; } // ⭐ Đã thêm: Thuộc tính đánh giá trung bình của sản phẩm

        [NotMapped]
        public int ReviewCount { get; set; } // ⭐ Đã thêm: Thuộc tính số lượng đánh giá của sản phẩm

    }
}