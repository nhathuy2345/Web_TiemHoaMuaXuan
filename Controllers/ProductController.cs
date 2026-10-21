using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models.Entities;
using Web_TiemHoaMuaXuan.Services;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService = new ProductService();

        public ActionResult Index(int? categoryId, string search, string priceRange)
        {
            var products = _productService.GetAll();

            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId).ToList();

            if (!string.IsNullOrWhiteSpace(search))
                products = products.Where(p => p.Name.ToLower().Contains(search.ToLower())).ToList();

            // ⭐ ĐOẠN XỬ LÝ LỌC GIÁ
            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                switch (priceRange)
                {
                    case "under500":
                        products = products.Where(p => p.Price < 500000).ToList();
                        break;
                    case "500to1000":
                        products = products.Where(p => p.Price >= 500000 && p.Price <= 1000000).ToList();
                        break;
                    case "over1000":
                        products = products.Where(p => p.Price > 1000000).ToList();
                        break;
                }
            }

            ViewBag.Categories = _productService.GetCategories();
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.Search = search;
            ViewBag.CurrentPriceRange = priceRange; // Quan trọng: Truyền lại để nút đổi màu

            return View(products);
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Index");

            var product = _productService.GetById(id.Value);
            if (product == null) return HttpNotFound();

            var reviewService = new ReviewService();

            // Truyền dữ liệu đánh giá sang View
            ViewBag.Reviews = reviewService.GetReviews(id.Value);
            ViewBag.AverageRating = reviewService.GetAverageRating(id.Value);
            ViewBag.ReviewCount = ((List<Web_TiemHoaMuaXuan.Models.Entities.Review>)ViewBag.Reviews).Count;

            ViewBag.Categories = _productService.GetCategories();
            ViewBag.Related = _productService.GetAll()
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id.Value)
                .Take(4).ToList();

            return View(product);
        }

        // POST: Thêm đánh giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(int productId, int rating, string comment)
        {
            var user = Session["User"] as Web_TiemHoaMuaXuan.Models.Entities.AppUser;

            // Bắt buộc phải đăng nhập mới được đánh giá
            if (user == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để đánh giá sản phẩm!";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", "Product", new { id = productId }) });
            }

            if (rating < 1 || rating > 5 || string.IsNullOrWhiteSpace(comment))
            {
                TempData["Error"] = "Vui lòng chọn số sao và nhập bình luận.";
                return RedirectToAction("Details", new { id = productId });
            }

            var reviewService = new ReviewService();
            reviewService.AddReview(new Web_TiemHoaMuaXuan.Models.Entities.Review
            {
                ProductId = productId,
                UserEmail = user.Email,
                UserName = user.FullName,
                Rating = rating,
                Comment = comment
            });

            TempData["Success"] = "Cảm ơn bạn đã đánh giá sản phẩm!";
            return RedirectToAction("Details", new { id = productId });
        }
    }
}