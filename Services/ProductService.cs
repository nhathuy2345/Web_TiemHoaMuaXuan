using System.Collections.Generic;
using System.Linq;
using Web_TiemHoaMuaXuan.Models;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Services
{
    /// <summary>
    /// Service xử lý Product — tách logic DB khỏi Controller
    /// </summary>
    public class ProductService
    {
        private readonly TiemHoaMuaXuanDbContext _db = new TiemHoaMuaXuanDbContext();

        public List<Product> GetAll()
        {
            return _db.Products.OrderBy(p => p.Id).ToList();
        }

        public List<Product> GetFeatured(int take = 6)
        {
            return _db.Products
                .Where(p => p.IsFeatured)
                .OrderBy(p => p.Id)
                .Take(take)
                .ToList();
        }

        public List<Product> GetByCategory(int categoryId)
        {
            return _db.Products
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Id)
                .ToList();
        }

        public Product GetById(int id)
        {
            return _db.Products.FirstOrDefault(p => p.Id == id);
        }

        public List<Category> GetCategories()
        {
            return _db.Categories.Where(c => c.IsActive).OrderBy(c => c.Id).ToList();
        }

        public Category GetCategory(int id)
        {
            return _db.Categories.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Product product)
        {
            _db.Products.Add(product);
            _db.SaveChanges();
        }

        public void Update(Product product)
        {
            var existing = _db.Products.Find(product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Price = product.Price;
                existing.OldPrice = product.OldPrice;
                existing.DiscountPercent = product.DiscountPercent;
                existing.ImageUrl = product.ImageUrl;
                existing.Stock = product.Stock;
                existing.CategoryId = product.CategoryId;
                existing.IsFeatured = product.IsFeatured;
                _db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var product = _db.Products.Find(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                _db.SaveChanges();
            }
        }

        public int Count()
        {
            return _db.Products.Count();
        }
        // =====================================================
        // CÁC HÀM XỬ LÝ DANH MỤC (Dành cho trang Admin)
        // =====================================================

        // Lấy TẤT CẢ danh mục (kể cả những danh mục đang bị ẩn) để hiển thị bên Admin
        public List<Category> GetAllCategories()
        {
            return _db.Categories.OrderBy(c => c.Id).ToList();
        }

        public Category GetCategoryById(int id)
        {
            return _db.Categories.FirstOrDefault(c => c.Id == id);
        }

        public void AddCategory(Category category)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            var existing = _db.Categories.Find(category.Id);
            if (existing != null)
            {
                existing.Name = category.Name;
                existing.Description = category.Description;
                existing.IsActive = category.IsActive;
                _db.SaveChanges();
            }
        }

        public void DeleteCategory(int id)
        {
            var category = _db.Categories.Find(id);
            if (category != null)
            {
                _db.Categories.Remove(category);
                _db.SaveChanges();
            }
        }

        public bool HasProducts(int categoryId)
        {
            // Trả về true nếu có sản phẩm thuộc danh mục này
            return _db.Products.Any(p => p.CategoryId == categoryId);
        }
    }
}