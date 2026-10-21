using System.Web;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models.Entities;

namespace Web_TiemHoaMuaXuan.Controllers
{
    public class AccountController : Controller
    {
        // 1. GET: Hiển thị form đăng nhập
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // 2. POST: Xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password, string returnUrl)
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

            // Lưu User vào Session
            Session["User"] = user;

            TempData["Success"] = "Đăng nhập thành công! Chào mừng " + user.FullName + " trở lại.";

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

            TempData["Success"] = "Đăng ký thành công! Chào mừng " + user.FullName + " đến với SpringFlower.";

            return RedirectToAction("Index", "Home");
        }

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

            // Đọc đơn hàng từ DB
            var orderService = new Web_TiemHoaMuaXuan.Services.OrderService();
            var orders = orderService.GetByEmail(user.Email);

            ViewBag.Orders = orders;
            return View(user);
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
            var user = Session["User"] as AppUser;
            if (user == null) return RedirectToAction("Login");

            // Cập nhật lại thông tin vào Session
            user.FullName = fullName;
            user.Phone = phone;
            user.Address = address;

            // Lưu lại vào Session
            Session["User"] = user;

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile");
        }

        // POST: /Account/CancelOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(int id)
        {
            var user = Session["User"] as AppUser;
            if (user == null) return RedirectToAction("Login");

            var orderService = new Web_TiemHoaMuaXuan.Services.OrderService();
            var order = orderService.GetById(id);

            // 1. Kiểm tra đơn hàng có tồn tại không
            if (order == null) return HttpNotFound();

            // 2. Bảo mật: Chỉ được hủy đơn của chính mình
            if (order.UserEmail != user.Email)
            {
                TempData["Error"] = "Bạn không có quyền hủy đơn hàng này.";
                return RedirectToAction("Profile");
            }

            // 3. Nghiệp vụ: Chỉ được hủy khi đơn đang "Chờ xử lý"
            if (order.Status == "Chờ xử lý")
            {
                // Cập nhật trạng thái thành Đã hủy
                orderService.UpdateStatus(id, "Đã hủy");

                // ⭐ CỘNG LẠI TỒN KHO ĐÃ SỬA ⭐
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