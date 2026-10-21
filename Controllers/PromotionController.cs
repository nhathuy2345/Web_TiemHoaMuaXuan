using System;
using System.Linq;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models.Entities;
using Web_TiemHoaMuaXuan.Services;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class PromotionController : Controller
    {
        private readonly ProductService _productService = new ProductService();

        // GET: /Promotion
        public ActionResult Index()
        {
            ViewBag.Title = "Khuyến mãi";

            // Lấy tất cả SP có giảm giá từ DB
            var promotions = _productService.GetAll()
                .Where(p => p.DiscountPercent > 0)
                .OrderByDescending(p => p.DiscountPercent)
                .ToList();

            return View(promotions);
        }

        // GET: /Promotion/Voucher
        public ActionResult Voucher()
        {
            ViewBag.Title = "Kho Voucher";

            // ⭐ Đảm bảo dùng CouponData (RAM) thay vì CouponService
            var vouchers = CouponData.Coupons
                .Where(c => c.IsActive && c.ExpiryDate > DateTime.Now)
                .OrderBy(c => c.DiscountAmount)
                .ToList();

            return View(vouchers);
        }
    }
}