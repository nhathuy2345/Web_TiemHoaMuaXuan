using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models.Entities;
using System.Data.Entity;
using Web_TiemHoaMuaXuan.Models;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class AccountController : Controller
    {
        // Khai báo DbContext để làm việc với Database
        private readonly Web_TiemHoaMuaXuan.Models.TiemHoaMuaXuanDbContext db = new Web_TiemHoaMuaXuan.Models.TiemHoaMuaXuanDbContext();

        // =====================================================
        // ĐĂNG NHẬP (LOGIN)
        // =====================================================
        // GET: Hiển thị form đăng nhập
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            // Tìm user trong Database
            var user = db.AppUsers.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Email hoặc mật khẩu không chính xác.";
                return View();
            }

            if (!user.IsActive)
            {
                ViewBag.Error = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Admin.";
                return View();
            }

            // Lưu User vào Session
            Session["User"] = user;
            TempData["Success"] = "Đăng nhập thành công! Chào mừng " + user.FullName + " trở lại.";

            if (user.IsAdmin)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // =====================================================
        // ĐĂNG KÝ (REGISTER)
        // =====================================================
        // GET: Hiển thị form đăng ký
        public ActionResult Register()
        {
            return View();
        }

        // POST: Xử lý đăng ký (Lưu vào Database)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string fullName, string email, string phone, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp";
                return View();
            }

            // Kiểm tra email đã tồn tại chưa
            if (db.AppUsers.Any(u => u.Email == email))
            {
                ViewBag.Error = "Email này đã được sử dụng. Vui lòng chọn email khác.";
                return View();
            }

            var user = new AppUser
            {
                Email = email,
                Password = password,
                FullName = fullName,
                Phone = phone,
                IsAdmin = false, // Mặc định đăng ký là khách
                IsActive = true
            };

            db.AppUsers.Add(user);
            db.SaveChanges();

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // =====================================================
        // ĐĂNG XUẤT & TRANG CÁ NHÂN
        // =====================================================
        public ActionResult Logout()
        {
            Session.Remove("User");
            TempData["Success"] = "Bạn đã đăng xuất khỏi hệ thống.";
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Profile()
        {
            var user = Session["User"] as AppUser;
            if (user == null) return RedirectToAction("Login");

            var orderService = new Web_TiemHoaMuaXuan.Services.OrderService();
            var orders = orderService.GetByEmail(user.Email);

            ViewBag.Orders = orders;
            return View(user);
        }
        // GET: /Account/OrderDetail/5
        public ActionResult OrderDetail(int id)
        {
            var user = Session["User"] as AppUser;
            if (user == null) return RedirectToAction("Login");

            var orderService = new Web_TiemHoaMuaXuan.Services.OrderService();
            var order = orderService.GetById(id);

            // ⭐ BẢO MẬT: Chỉ cho phép xem đơn hàng của chính mình
            if (order == null || order.UserEmail != user.Email)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng hoặc bạn không có quyền truy cập.";
                return RedirectToAction("Profile");
            }

            ViewBag.Title = "Chi tiết đơn hàng #" + order.OrderCode;
            return View(order);
        }

        // GET: /Account/EditProfile
        public ActionResult EditProfile()
        {
            var user = Session["User"] as AppUser;
            if (user == null) return RedirectToAction("Login");

            return View(user);
        }

        // POST: /Account/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(string fullName, string phone, string address)
        {
            var sessionUser = Session["User"] as AppUser;
            if (sessionUser == null) return RedirectToAction("Login");

            // ⭐ Lấy user từ Database ra để cập nhật
            var dbUser = db.AppUsers.Find(sessionUser.Id);
            if (dbUser != null)
            {
                dbUser.FullName = fullName;
                dbUser.Phone = phone;
                dbUser.Address = address;

                db.SaveChanges(); // Lưu thay đổi vào DB

                // Cập nhật lại thông tin vào Session
                Session["User"] = dbUser;
                TempData["Success"] = "Cập nhật thông tin thành công!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
            }

            return RedirectToAction("Profile");
        }

        // =====================================================
        // HỦY ĐƠN HÀNG (PHÍA KHÁCH HÀNG)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(int id)
        {
            var user = Session["User"] as AppUser;
            if (user == null) return RedirectToAction("Login");

            var orderService = new Web_TiemHoaMuaXuan.Services.OrderService();
            var order = orderService.GetById(id);

            if (order == null) return HttpNotFound();

            if (order.UserEmail != user.Email)
            {
                TempData["Error"] = "Bạn không có quyền hủy đơn hàng này.";
                return RedirectToAction("Profile");
            }

            if (order.Status == "Chờ xử lý")
            {
                orderService.UpdateStatus(id, "Đã hủy");
                orderService.RestoreStock(id);

                TempData["Success"] = "Đã hủy đơn hàng #" + order.OrderCode + " thành công.";
            }
            else
            {
                TempData["Error"] = "Đơn hàng đang ở trạng thái '" + order.Status + "' nên không thể hủy. Vui lòng liên hệ hotline.";
            }

            return RedirectToAction("Profile");
        }
    }
}