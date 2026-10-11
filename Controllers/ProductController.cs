using System.Linq;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult Index(int? categoryId, string search)
        {
            var products = MockData.Products;

            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId).ToList();

            if (!string.IsNullOrWhiteSpace(search))
                products = products.Where(p => p.Name.ToLower().Contains(search.ToLower())).ToList();

            ViewBag.Categories = MockData.Categories;
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.Search = search;
            return View(products);
        }

        public ActionResult Details(int id)
        {
            var product = MockData.GetProduct(id);
            if (product == null) return HttpNotFound();

            ViewBag.Categories = MockData.Categories;
            ViewBag.Related = MockData.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                .Take(3).ToList();
            return View(product);
        }
    }
}