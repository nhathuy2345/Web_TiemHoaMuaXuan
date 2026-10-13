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
    }
}