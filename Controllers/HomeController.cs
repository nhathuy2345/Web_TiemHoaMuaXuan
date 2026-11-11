using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Services;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _productService = new ProductService();

        public ActionResult Index()
        {
            ViewBag.Categories = _productService.GetCategories();
            var featured = _productService.GetFeatured();
            return View(featured);
        }
        // GET: /Home/About
        public ActionResult About()
        {
            ViewBag.Title = "Giới thiệu";
            return View();
        }
        // GET: /Home/News
        public ActionResult News()
        {
            ViewBag.Title = "Tin nổi bật";
            return View();
        }
        // GET: /Home/Store
        public ActionResult Store()
        {
            ViewBag.Title = "Hệ thống cửa hàng";
            return View();
        }
        public ActionResult PrivacyPolicy()
        {
            ViewBag.Title = "Chính sách bảo mật";
            return View();
        }

        public ActionResult TermsOfService()
        {
            ViewBag.Title = "Điều khoản sử dụng";
            return View();
        }
    }
}