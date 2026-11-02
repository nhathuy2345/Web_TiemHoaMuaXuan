namespace Web_TiemHoaMuaXuan.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Web_TiemHoaMuaXuan.Models.Entities;

    internal sealed class Configuration
        : DbMigrationsConfiguration<Web_TiemHoaMuaXuan.Models.TiemHoaMuaXuanDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Web_TiemHoaMuaXuan.Models.TiemHoaMuaXuanDbContext context)
        {
            // ===== SEED CATEGORIES =====
            if (!context.Categories.Any())
            {
                context.Categories.AddOrUpdate(c => c.Name,
                    new Category { Name = "Hoa Tình Yêu", Description = "Hoa cho người yêu", IsActive = true },
                    new Category { Name = "Hoa Sinh Nhật", Description = "Hoa chúc mừng sinh nhật", IsActive = true },
                    new Category { Name = "Hoa Khai Trương", Description = "Hoa chúc mừng khai trương", IsActive = true },
                    new Category { Name = "Hoa Cưới", Description = "Hoa cầm tay cô dâu", IsActive = true },
                    new Category { Name = "Lan Hồ Điệp", Description = "Lan hồ điệp cao cấp", IsActive = true }
                );
                context.SaveChanges();
            }

            // Lấy Id các category đã insert
            var cat1 = context.Categories.First(c => c.Name == "Hoa Tình Yêu").Id;
            var cat2 = context.Categories.First(c => c.Name == "Hoa Sinh Nhật").Id;
            var cat3 = context.Categories.First(c => c.Name == "Hoa Khai Trương").Id;
            var cat4 = context.Categories.First(c => c.Name == "Hoa Cưới").Id;
            var cat5 = context.Categories.First(c => c.Name == "Lan Hồ Điệp").Id;

            // ===== SEED PRODUCTS =====
            if (!context.Products.Any())
            {
                context.Products.AddOrUpdate(p => p.Name,
                    new Product { Name = "Bó Hoa Hướng Dương", Price = 450000, OldPrice = 500000, DiscountPercent = 10, Stock = 50, CategoryId = cat2, IsFeatured = true, ImageUrl = "/Content/images/1.jpg", Description = "Hoa hướng dương tươi 20 cành" },
                    new Product { Name = "Lẵng Hoa Hồng Đỏ", Price = 650000, Stock = 30, CategoryId = cat1, IsFeatured = true, ImageUrl = "/Content/images/2.jpg", Description = "99 bông hồng đỏ nhập khẩu" },
                    new Product { Name = "Chậu Lan Hồ Điệp Trắng", Price = 1200000, Stock = 20, CategoryId = cat5, IsFeatured = true, ImageUrl = "/Content/images/3.jpg", Description = "Lan hồ điệp trắng 5 cành" },
                    new Product { Name = "Hoa Cưới Cầm Tay", Price = 850000, Stock = 15, CategoryId = cat4, IsFeatured = true, ImageUrl = "/Content/images/4.jpg", Description = "Hoa cầm tay cô dâu" },
                    new Product { Name = "Kệ Hoa Khai Trương", Price = 2500000, OldPrice = 2800000, DiscountPercent = 11, Stock = 10, CategoryId = cat3, IsFeatured = true, ImageUrl = "/Content/images/5.jpg", Description = "Kệ hoa 2 tầng" },
                    new Product { Name = "Bó Hồng Pastel", Price = 520000, Stock = 40, CategoryId = cat1, ImageUrl = "/Content/images/6.jpg", Description = "Hồng pastel nhẹ nhàng" },
                    new Product { Name = "Giỏ Hoa Baby", Price = 380000, Stock = 60, CategoryId = cat2, ImageUrl = "/Content/images/7.jpg", Description = "Hoa baby trắng" },
                    new Product { Name = "Hộp Hoa Hồng Vàng", Price = 720000, Stock = 25, CategoryId = cat1, ImageUrl = "/Content/images/8.jpg", Description = "Hộp quà hoa hồng vàng" },
                    new Product { Name = "Lan Hồ Điệp Tím", Price = 1500000, Stock = 8, CategoryId = cat5, ImageUrl = "/Content/images/9.jpg", Description = "Lan hồ điệp tím 7 cành" },
                    new Product { Name = "Hoa Cưới Tone Xanh", Price = 950000, Stock = 12, CategoryId = cat4, ImageUrl = "/Content/images/10.jpg", Description = "Hoa cầm tay tone xanh mint" },
                    new Product { Name = "Bó Tulip Hà Lan", Price = 680000, Stock = 35, CategoryId = cat2, ImageUrl = "/Content/images/11.jpg", Description = "Tulip Hà Lan tươi 15 cành" },
                    new Product { Name = "Chậu Hoa Cẩm Tú Cầu", Price = 890000, Stock = 18, CategoryId = cat3, ImageUrl = "/Content/images/12.jpg", Description = "Cẩm tú cầu xanh pastel" }
                );
                context.SaveChanges();
            }

            // ===== SEED 1 TIN NHẮN MẪU =====
            if (!context.ContactMessages.Any())
            {
                context.ContactMessages.Add(new ContactMessage
                {
                    Name = "Nguyễn Văn A",
                    Email = "vana@gmail.com",
                    Phone = "0901234567",
                    Message = "Shop có giao hoa vào Chủ nhật không?",
                    SentAt = DateTime.Now.AddHours(-2),
                    IsRead = false
                });
                context.SaveChanges();
            }
        }
    }
}