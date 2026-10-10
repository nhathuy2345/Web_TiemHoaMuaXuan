using System.Web.Mvc;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Dashboard()
        {
            ViewBag.Title = "Dashboard";
            ViewBag.TotalProducts = Models.MockData.Products.Count;
            ViewBag.TotalCategories = Models.MockData.Categories.Count;
            ViewBag.TotalOrders = 15;
            ViewBag.Revenue = 25000000m;
            return View();
        }

        public ActionResult ProductManager()
        {
            ViewBag.Title = "Quản lý sản phẩm";
            var products = Models.MockData.Products;
            return View(products);
        }

        public ActionResult OrderManager()
        {
            ViewBag.Title = "Quản lý đơn hàng";
            return View();
        }
    }
}