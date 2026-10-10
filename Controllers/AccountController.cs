using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            var user = new AppUser
            {
                Email = email,
                FullName = email.Split('@')[0],
                IsAdmin = email.ToLower().Contains("admin")
            };

            Session["User"] = user;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string fullName, string email, string phone,
                                     string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp";
                return View();
            }

            var user = new AppUser
            {
                Email = email,
                FullName = fullName,
                Phone = phone,
                IsAdmin = false
            };

            Session["User"] = user;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Remove("User");
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Profile()
        {
            var user = Session["User"] as AppUser;
            if (user == null) return RedirectToAction("Login");
            return View(user);
        }
    }
}