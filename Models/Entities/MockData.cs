using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public static class MockData
    {
        public static List<Category> Categories => new List<Category>
        {
            new Category { Id = 1, Name = "Hoa Tình Yêu",    Description = "Hoa cho người yêu" },
            new Category { Id = 2, Name = "Hoa Sinh Nhật",   Description = "Hoa chúc mừng sinh nhật" },
            new Category { Id = 3, Name = "Hoa Khai Trương", Description = "Hoa chúc mừng khai trương" },
            new Category { Id = 4, Name = "Hoa Cưới",        Description = "Hoa cầm tay cô dâu" },
            new Category { Id = 5, Name = "Lan Hồ Điệp",     Description = "Lan hồ điệp cao cấp" }
        };

        public static List<Product> Products => new List<Product>
{
    new Product { Id = 1,  Name = "Bó Hoa Hướng Dương",    Price = 450000,  OldPrice = 500000,  DiscountPercent = 10, Stock = 50, CategoryId = 2, IsFeatured = true,
        ImageUrl = "/Content/images/huong duong.jpg", Description = "Hoa hướng dương tươi 20 cành" },
    new Product { Id = 2,  Name = "Bó Hoa Hồng Đỏ",       Price = 650000,  Stock = 30, CategoryId = 1, IsFeatured = true,
        ImageUrl = "/Content/images/hoa hong do.jpg", Description = "99 bông hồng đỏ nhập khẩu" },
    new Product { Id = 3,  Name = "Chậu Lan Hồ Điệp Trắng", Price = 1200000, Stock = 20, CategoryId = 5, IsFeatured = true,
        ImageUrl = "/Content/images/chau lan ho diep trang.jpg", Description = "Lan hồ điệp trắng 5 cành" },
    new Product { Id = 4,  Name = "Hoa Cưới Cầm Tay",       Price = 850000,  Stock = 15, CategoryId = 4, IsFeatured = true,
        ImageUrl = "/Content/images/hoa cuoi cam tay.jpg", Description = "Hoa cầm tay cô dâu tone trắng hồng" },
    new Product { Id = 5,  Name = "Kệ Hoa Khai Trương",     Price = 2500000, OldPrice = 2800000, DiscountPercent = 11, Stock = 10, CategoryId = 3, IsFeatured = true,
        ImageUrl = "/Content/images/ke hoa khai truong.jpg", Description = "Kệ hoa 2 tầng chúc mừng" },
    new Product { Id = 6,  Name = "Bó Hồng Pastel",         Price = 520000,  Stock = 40, CategoryId = 1,
        ImageUrl = "/Content/images/hoa-hong-pastel.jpg", Description = "Hồng pastel nhẹ nhàng" },
    new Product { Id = 7,  Name = "Giỏ Hoa Baby",           Price = 380000,  Stock = 60, CategoryId = 2,
        ImageUrl = "/Content/images/gio-hoa-baby.jpg", Description = "Hoa baby trắng trong giỏ mây" },
    new Product { Id = 8,  Name = "Hộp Hoa Hồng Vàng",      Price = 720000,  Stock = 25, CategoryId = 1,
        ImageUrl = "/Content/images/hop-hoa-hong-vang.jpg", Description = "Hộp quà hoa hồng vàng" },
    new Product { Id = 9,  Name = "Lan Hồ Điệp Tím",        Price = 1500000, Stock = 8,  CategoryId = 5,
        ImageUrl = "/Content/images/lan-ho-diep-tim.jpg", Description = "Lan hồ điệp tím 7 cành" },
    new Product { Id = 10, Name = "Hoa Cưới Tone Xanh",     Price = 950000,  Stock = 12, CategoryId = 4,
        ImageUrl = "/Content/images/hoa-cuoi-tone-xanh.jpg", Description = "Hoa cầm tay tone xanh mint" },
    new Product { Id = 11, Name = "Bó Tulip Hà Lan",        Price = 680000,  Stock = 35, CategoryId = 2,
        ImageUrl = "/Content/images/tulip-ha-lan.jpg", Description = "Tulip Hà Lan tươi 15 cành" },
    new Product { Id = 12, Name = "Chậu Hoa Cẩm Tú Cầu",    Price = 890000,  Stock = 18, CategoryId = 3,
        ImageUrl = "/Content/images/cam-tu-cau.jpg", Description = "Cẩm tú cầu xanh pastel" }
};

        public static Product GetProduct(int id) => Products.FirstOrDefault(p => p.Id == id);
        public static Category GetCategory(int id) => Categories.FirstOrDefault(c => c.Id == id);
        public static List<Product> GetByCategory(int categoryId) => Products.Where(p => p.CategoryId == categoryId).ToList();
        public static List<Product> GetFeatured() => Products.Where(p => p.IsFeatured).Take(6).ToList();
    }
}