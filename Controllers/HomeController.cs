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
    }
}