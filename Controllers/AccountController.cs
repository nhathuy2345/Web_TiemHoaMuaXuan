using System.Web;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string fullName, string email, string phone,
                                     string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ họ tên và email";
                return View();
            }

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

        // GET: /Account/Logout
        public ActionResult Logout()
        {
            Session.Remove("User");
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Profile
        public ActionResult Profile()
        {
            var user = Session["User"] as AppUser;
            if (user == null) return RedirectToAction("Login");
            return View(user);
        }
    }
}