using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                // Xử lý đăng nhập thành công
                ViewBag.Message = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string fullname, string email, string phone, string password)
        {
            if (ModelState.IsValid)
            {
                // Xử lý đăng ký tài khoản thành công
                return RedirectToAction("Login");
            }
            return View();
        }

        // GET: Account/Profile
        public ActionResult Profile()
        {
            // Dữ liệu mẫu trang cá nhân
            ViewBag.FullName = "Nguyễn Đình Gia Bảo";
            ViewBag.Email = "giabao@example.com";
            ViewBag.Phone = "0901234567";
            ViewBag.Address = "TP. Hồ Chí Minh";

            return View();
        }
    }
}