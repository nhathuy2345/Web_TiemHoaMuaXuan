using System;
using System.Collections.Generic;
using System.Linq;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public static class MockData
    {
        // ===== DANH MỤC =====
        public static List<Category> Categories = new List<Category>
        {
            new Category { Id = 1, Name = "Hoa Tình Yêu",    Description = "Hoa cho người yêu" },
            new Category { Id = 2, Name = "Hoa Sinh Nhật",   Description = "Hoa chúc mừng sinh nhật" },
            new Category { Id = 3, Name = "Hoa Khai Trương", Description = "Hoa chúc mừng khai trương" },
            new Category { Id = 4, Name = "Hoa Cưới",        Description = "Hoa cầm tay cô dâu" },
            new Category { Id = 5, Name = "Lan Hồ Điệp",     Description = "Lan hồ điệp cao cấp" }
        };

        // ===== SẢN PHẨM (dạng static field để add/remove được) =====
        public static List<Product> Products = new List<Product>
        {
            new Product { Id = 1,  Name = "Bó Hoa Hướng Dương",    Price = 450000,  OldPrice = 500000,  DiscountPercent = 10, Stock = 50, CategoryId = 2, IsFeatured = true,  ImageUrl = "/Content/images/huong duong.jpg",  Description = "Hoa hướng dương tươi 20 cành" },
            new Product { Id = 2,  Name = "Lẵng Hoa Hồng Đỏ",       Price = 650000,  Stock = 30, CategoryId = 1, IsFeatured = true,  ImageUrl = "/Content/images/hoa hong do.jng",  Description = "99 bông hồng đỏ nhập khẩu" },
            new Product { Id = 3,  Name = "Chậu Lan Hồ Điệp Trắng", Price = 1200000, Stock = 20, CategoryId = 5, IsFeatured = true,  ImageUrl = "/Content/images/chau lan ho diep trang.jpg",  Description = "Lan hồ điệp trắng 5 cành" },
            new Product { Id = 4,  Name = "Hoa Cưới Cầm Tay",       Price = 850000,  Stock = 15, CategoryId = 4, IsFeatured = true,  ImageUrl = "/Content/images/hoa cuoi cam tay.jpg",  Description = "Hoa cầm tay cô dâu" },
            new Product { Id = 5,  Name = "Kệ Hoa Khai Trương",     Price = 2500000, OldPrice = 2800000, DiscountPercent = 11, Stock = 10, CategoryId = 3, IsFeatured = true,  ImageUrl = "/Content/images/ke hoa khai truong.jpg",  Description = "Kệ hoa 2 tầng" },
            new Product { Id = 6,  Name = "Bó Hồng Pastel",         Price = 520000,  Stock = 40, CategoryId = 1, ImageUrl = "/Content/images/hoahongpasteltrang.jpg",  Description = "Hồng pastel nhẹ nhàng" },
            new Product { Id = 7,  Name = "Giỏ Hoa Baby",           Price = 380000,  Stock = 60, CategoryId = 2, ImageUrl = "/Content/images/7.jpg",  Description = "Hoa baby trắng" },
            new Product { Id = 8,  Name = "Hộp Hoa Hồng Vàng",      Price = 720000,  Stock = 25, CategoryId = 1, ImageUrl = "/Content/images/hop qua hoa hong vang.jpg",  Description = "Hộp quà hoa hồng vàng" },
            new Product { Id = 9,  Name = "Lan Hồ Điệp Tím",        Price = 1500000, Stock = 8,  CategoryId = 5, ImageUrl = "/Content/images/lan ho diep tim 7 canh.jpg",  Description = "Lan hồ điệp tím 7 cành" },
        };

        public static List<Order> Orders = new List<Order>();

        // ===== HELPER FUNCTIONS =====

        public static Product GetProduct(int id)
            => Products.FirstOrDefault(p => p.Id == id);

        public static Category GetCategory(int id)
            => Categories.FirstOrDefault(c => c.Id == id);

        public static List<Product> GetByCategory(int categoryId)
            => Products.Where(p => p.CategoryId == categoryId).ToList();

        public static List<Product> GetFeatured()
            => Products.Where(p => p.IsFeatured).Take(6).ToList();

        // Tự sinh Id mới cho Product
        public static int NextProductId()
            => Products.Count > 0 ? Products.Max(p => p.Id) + 1 : 1;

        // Tự sinh OrderCode
        public static string NextOrderCode()
        {
            int maxId = Orders.Count > 0 ? Orders.Max(o => o.Id) : 0;
            return "DH" + (1000 + maxId + 1);
        }

        // Lấy đơn theo email user
        public static List<Order> GetOrdersByEmail(string email)
            => Orders.Where(o => o.UserEmail == email).OrderByDescending(o => o.OrderDate).ToList();

        // Lấy 1 đơn theo Id
        public static Order GetOrder(int id)
            => Orders.FirstOrDefault(o => o.Id == id);
    }
}