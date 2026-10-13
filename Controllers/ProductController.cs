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

        public ActionResult Index(int? categoryId, string search)
        {
            List<Product> products;

            if (categoryId.HasValue)
                products = _productService.GetByCategory(categoryId.Value);
            else
                products = _productService.GetAll();

            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products
                    .Where(p => p.Name.ToLower().Contains(search.ToLower()))
                    .ToList();
            }

            ViewBag.Categories = _productService.GetCategories();
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.Search = search;
            return View(products);
        }

        public ActionResult Details(int id)
        {
            var product = _productService.GetById(id);
            if (product == null) return HttpNotFound();

            ViewBag.Categories = _productService.GetCategories();
            ViewBag.Related = _productService.GetAll()
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                .Take(3).ToList();
            return View(product);
        }
    }
}