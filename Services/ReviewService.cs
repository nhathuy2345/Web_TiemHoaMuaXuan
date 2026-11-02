using System.Collections.Generic;
using System.Linq;
using Web_TiemHoaMuaXuan.Models;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Services
{
    public class ReviewService
    {
        private readonly TiemHoaMuaXuanDbContext _db = new TiemHoaMuaXuanDbContext();

        // Lấy tất cả đánh giá của 1 sản phẩm (mới nhất xếp trước)
        public List<Review> GetReviews(int productId)
        {
            return _db.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        // Tính điểm trung bình (VD: 4.5)
        public double GetAverageRating(int productId)
        {
            var reviews = _db.Reviews.Where(r => r.ProductId == productId);
            if (!reviews.Any()) return 0;
            return reviews.Average(r => r.Rating);
        }

        // Thêm đánh giá mới
        public void AddReview(Review review)
        {
            _db.Reviews.Add(review);
            _db.SaveChanges();
        }
    }
}