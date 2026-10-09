using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Web_TiemHoaMuaXuan.Models;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Categories = MockData.Categories;
            var featured = MockData.GetFeatured();
            return View(featured);
        }
    }
}