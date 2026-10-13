using System.Linq;
using System.Web.Mvc;
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
    }
}