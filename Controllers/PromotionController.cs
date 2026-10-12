using System.Linq;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class PromotionController : Controller
    {
        // GET: /Promotion
        public ActionResult Index()
        {
            ViewBag.Title = "Khuyến mãi";

            // Lấy tất cả SP có giảm giá
            var promotions = MockData.Products
                .Where(p => p.DiscountPercent > 0)
                .OrderByDescending(p => p.DiscountPercent)
                .ToList();

            return View(promotions);
        }
    }
}