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
            // ⭐ Sửa thành OrderByDescending để sản phẩm mới nhất (ID lớn nhất) lên đầu
            var products = _db.Products.OrderByDescending(p => p.Id).ToList();
            CalculateRating(products);
            return products;

        }
        public List<Product> GetFeatured(int take = 6)
        {
            var products = _db.Products
                .Where(p => p.IsFeatured)
                .OrderBy(p => p.Id)
                .Take(take)
                .ToList();
            CalculateRating(products);
            return products;
        }

        public List<Product> GetByCategory(int categoryId)
        {
            var products = _db.Products
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Id)
                .ToList();
            CalculateRating(products);
            return products;
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
        private void CalculateRating(List<Product> products)
        {
            var reviewService = new ReviewService();
            foreach ( var p in products)
            {
                var reviews = reviewService.GetReviews(p.Id);// gọi đến ReviewService để lấy danh sách đánh giá của sản phẩm
                p.ReviewCount = reviews.Count;// đếm xem có bao nhiêu đánh giá
                if(reviews.Count > 0)// nếu có ít nhất 1 đánh giá
                {
                    double tongSoSao = 0;
                    foreach (var r in reviews)
                    {
                        tongSoSao= tongSoSao + r.Rating;// cộng dồn số sao
                    }
                    p.AverageRating = tongSoSao / reviews.Count;// chia đều
                }
                else
                {
                    p.AverageRating = 0;// nếu chưa có đánh giá nào thì gán = 0
                }
            }    
        }
    }
}