using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Models;
using Web_TiemHoaMuaXuan.Models.Entities;

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
        // GET: Hiển thị form đăng nhập
        public ActionResult Login(string returnUrl)
        {
            // ⭐ 1. Nếu người dùng đã đăng nhập rồi thì không cho vào trang này nữa
            var user = Session["User"] as AppUser;
            if (user != null)
            {
                // Nếu là Admin thì về Dashboard, khách thì về Trang chủ
                if (user.IsAdmin)
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Index", "Home");
            }

            // ⭐ 2. Xóa các thông báo rác cũ (nếu có) để không bị lẫn lộn
            TempData["Success"] = null;
            TempData["Error"] = null;
            ViewBag.Error = null;

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
        // POST: Xử lý đăng ký (Lưu vào Database)
        // POST: Xử lý đăng ký (Lưu vào Database)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string fullName, string email, string phone, string password, string confirmPassword, string address)
        {
            //hàm hỗ trợ: lưu lại dữ liệu vào ViewBag trước khi trả về view
            void SaveDataToViewBag()
            {
                ViewBag.FullName = fullName;
                ViewBag.Email = email;
                ViewBag.Phone = phone;
                ViewBag.Address = address;
            }
            // 1. Kiểm tra mật khẩu khớp
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            // 2. KIỂM TRA HỌ TÊN (Không khoảng trắng đầu, tối đa 50 ký tự)
            if (string.IsNullOrWhiteSpace(fullName) || fullName.StartsWith(" ") || fullName.Length > 50)
            {
                ViewBag.Error = "Họ tên không hợp lệ (không được có khoảng trắng ở đầu, tối đa 50 ký tự).";
                return View();
            }

            // 3. KIỂM TRA SỐ ĐIỆN THOẠI (Chỉ chứa số, độ dài 10-11 ký tự)
            if (string.IsNullOrWhiteSpace(phone) || !System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{10,11}$"))
            {
                ViewBag.Error = "Số điện thoại không hợp lệ (Phải là 10 hoặc 11 chữ số).";
                return View();
            }

            // 4. KIỂM TRA MẬT KHẨU (Viết hoa chữ đầu, 4-12 ký tự, không có khoảng trắng đầu)
            if (string.IsNullOrWhiteSpace(password) || password.StartsWith(" "))
            {
                ViewBag.Error = "Mật khẩu không được bắt đầu bằng khoảng trắng.";
                return View();
            }
            if (password.Length < 4 || password.Length > 12)
            {
                ViewBag.Error = "Mật khẩu phải từ 4 đến 12 ký tự.";
                return View();
            }
            if (!char.IsUpper(password[0]))
            {
                ViewBag.Error = "Mật khẩu phải viết hoa chữ cái đầu tiên.";
                return View();
            }

            // 5. Kiểm tra email đã tồn tại chưa
            if (db.AppUsers.Any(u => u.Email == email))
            {
                ViewBag.Error = "Email này đã được sử dụng. Vui lòng chọn email khác.";
                return View();
            }

            // 6. Lưu vào Database
            var user = new AppUser
            {
                Email = email,
                Password = password,
                FullName = fullName,
                Phone = phone,
                Address = address,
                IsAdmin = false,
                IsActive = true
            };

            try
            {
                db.AppUsers.Add(user);
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Lấy ra tất cả các lỗi validation
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                // Nối chúng lại thành một chuỗi
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " Lỗi chi tiết: ", fullErrorMessage);

                // Gán vào ViewBag.Error để hiển thị ra màn hình
                ViewBag.Error = exceptionMessage;
                return View(); // Trả về form để xem lỗi
            }

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

            if (order == null || order.UserEmail != user.Email)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng hoặc bạn không có quyền truy cập.";
                return RedirectToAction("Profile");
            }

            // ⭐ THÊM ĐOẠN NÀY: Tạo lại mã QR nếu đơn đang chờ thanh toán
            if (order.Status == "Chờ thanh toán" && order.PaymentMethod == "Bank")
            {
                var bankService = new Web_TiemHoaMuaXuan.Services.BankSettingService();
                var bankInfo = bankService.Get();
                if (bankInfo != null)
                {
                    string accountNumber = bankInfo.AccountNumber;
                    string bankCode = bankInfo.BankName.Replace(" ", "");
                    string description = "Thanh toan don hang " + order.OrderCode;
                    ViewBag.QrUrl = $"https://qr.sepay.vn/img?acc={accountNumber}&bank={bankCode}&amount={(int)order.TotalAmount}&des={Uri.EscapeDataString(description)}";
                }
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

            if (order.Status == "Chờ xử lý" || order.Status == "Chờ thanh toán")
            {
                //cập nhật trạng thái đơn hàng thành "Đã hủy" và khôi phục số lượng sản phẩm trong kho
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
        // GET: Hiển thị form đăng ký
        public ActionResult Register()
        {
            return View();
        }
    }
}