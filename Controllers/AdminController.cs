using System;
using System.Linq;
using System.Web.Mvc;
using Web_TiemHoaMuaXuan.Filters;
using Web_TiemHoaMuaXuan.Models; // ⭐ Thêm dòng này để nhận diện DbContext
using Web_TiemHoaMuaXuan.Models.Entities;
using Web_TiemHoaMuaXuan.Services;

namespace Web_TiemHoaMuaXuan.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private readonly ProductService _productService = new ProductService();
        private readonly OrderService _orderService = new OrderService();

        // ⭐ KHAI BÁO BIẾN DB Ở ĐÂY (Thay TiemHoaMuaXuanDbContext bằng tên thực tế của bạn nếu khác)
        private readonly TiemHoaMuaXuanDbContext db = new TiemHoaMuaXuanDbContext();

        // =====================================================
        // DASHBOARD
        // =====================================================
        public ActionResult Dashboard()
        {
            ViewBag.Title = "Dashboard";
            ViewBag.TotalProducts = _productService.Count();
            ViewBag.TotalCategories = _productService.GetCategories().Count;
            ViewBag.TotalOrders = _orderService.Count();
            ViewBag.Revenue = _orderService.GetRevenue();
            return View();
        }

        // =====================================================
        // QUẢN LÝ SẢN PHẨM
        // =====================================================
        public ActionResult ProductManager()
        {
            ViewBag.Title = "Quản lý sản phẩm";
            var products = _productService.GetAll();
            return View(products);
        }

        public ActionResult CreateProduct()
        {
            ViewBag.Title = "Thêm sản phẩm";
            ViewBag.Categories = new SelectList(_productService.GetCategories(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(Product model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || model.Price <= 0)
            {
                ViewBag.Error = "Vui lòng nhập tên và giá sản phẩm";
                ViewBag.Categories = new SelectList(_productService.GetCategories(), "Id", "Name");
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.ImageUrl))
                model.ImageUrl = "/Content/images/1.jpg";

            _productService.Add(model);

            TempData["Success"] = "Đã thêm sản phẩm: " + model.Name;
            return RedirectToAction("ProductManager");
        }

        public ActionResult EditProduct(int id)
        {
            var product = _productService.GetById(id);
            if (product == null) return HttpNotFound();

            ViewBag.Title = "Sửa sản phẩm";
            ViewBag.Categories = new SelectList(_productService.GetCategories(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product model)
        {
            var existing = _productService.GetById(model.Id);
            if (existing == null) return HttpNotFound();

            if (string.IsNullOrWhiteSpace(model.Name) || model.Price <= 0)
            {
                ViewBag.Error = "Vui lòng nhập tên và giá sản phẩm";
                ViewBag.Categories = new SelectList(_productService.GetCategories(), "Id", "Name", model.CategoryId);
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.ImageUrl))
                model.ImageUrl = existing.ImageUrl;

            _productService.Update(model);

            TempData["Success"] = "Đã cập nhật sản phẩm: " + model.Name;
            return RedirectToAction("ProductManager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProduct(int id)
        {
            var product = _productService.GetById(id);
            if (product == null) return HttpNotFound();

            string name = product.Name;
            _productService.Delete(id);

            TempData["Success"] = "Đã xóa sản phẩm: " + name;
            return RedirectToAction("ProductManager");
        }

        // =====================================================
        // QUẢN LÝ ĐƠN HÀNG
        // =====================================================
        public ActionResult OrderManager()
        {
            ViewBag.Title = "Quản lý đơn hàng";
            var orders = _orderService.GetAll();
            return View(orders);
        }

        public ActionResult OrderDetail(int id)
        {
            var order = _orderService.GetById(id);
            if (order == null) return HttpNotFound();

            ViewBag.Title = "Chi tiết đơn hàng #" + order.OrderCode;
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOrderStatus(int id, string status)
        {
            var validStatuses = new[] { "Chờ xử lý", "Đã xác nhận", "Đang giao hàng", "Đã giao", "Đã hủy" };
            if (!validStatuses.Contains(status))
            {
                TempData["Error"] = "Trạng thái không hợp lệ";
                return RedirectToAction("OrderDetail", new { id = id });
            }

            var order = _orderService.GetById(id);
            if (order == null) return HttpNotFound();
            string oldStatus = order.Status;

            if (_orderService.UpdateStatus(id, status))
            {
                if (status == "Đã hủy" && oldStatus != "Đã hủy")
                {
                    _orderService.RestoreStock(id);
                }

                TempData["Success"] = $"Đơn {order.OrderCode}: {oldStatus} → {status}";
            }
            return RedirectToAction("OrderDetail", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickConfirm(int id)
        {
            var order = _orderService.GetById(id);
            if (order != null && order.Status == "Chờ xử lý")
            {
                _orderService.UpdateStatus(id, "Đã xác nhận");
                TempData["Success"] = "Đã xác nhận đơn " + order.OrderCode;
            }
            return RedirectToAction("OrderManager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickShip(int id)
        {
            var order = _orderService.GetById(id);
            if (order != null && order.Status == "Đã xác nhận")
            {
                _orderService.UpdateStatus(id, "Đang giao hàng");
                TempData["Success"] = "Đang giao đơn " + order.OrderCode;
            }
            return RedirectToAction("OrderManager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickDeliver(int id)
        {
            var order = _orderService.GetById(id);
            if (order != null && order.Status == "Đang giao hàng")
            {
                _orderService.UpdateStatus(id, "Đã giao");
                TempData["Success"] = "Đã giao đơn " + order.OrderCode;
            }
            return RedirectToAction("OrderManager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickCancel(int id)
        {
            var order = _orderService.GetById(id);
            if (order != null && order.Status != "Đã giao" && order.Status != "Đã hủy")
            {
                _orderService.UpdateStatus(id, "Đã hủy");
                _orderService.RestoreStock(id);

                TempData["Success"] = "Đã hủy đơn " + order.OrderCode + " và hoàn lại tồn kho.";
            }
            else
            {
                TempData["Error"] = "Không thể hủy đơn này";
            }
            return RedirectToAction("OrderManager");
        }

        // =====================================================
        // QUẢN LÝ TIN NHẮN LIÊN HỆ
        // =====================================================
        public ActionResult MessageManager()
        {
            ViewBag.Title = "Tin nhắn liên hệ";
            var service = new ContactService();
            var messages = service.GetAll();
            ViewBag.UnreadCount = service.CountUnread();
            return View(messages);
        }

        public ActionResult MessageDetail(int id)
        {
            var service = new ContactService();
            var message = service.GetById(id);
            if (message == null) return HttpNotFound();

            service.MarkAsRead(id);

            ViewBag.Title = "Chi tiết tin nhắn #" + message.Id;
            return View(message);
        }

        public ActionResult ChatManager()
        {
            ViewBag.Title = "Hỗ trợ trực tuyến";
            var chatService = new ChatService();
            ViewBag.ListCustomers = chatService.GetListUsersChat();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMessage(int id)
        {
            var service = new ContactService();
            var message = service.GetById(id);
            if (message != null)
            {
                service.Delete(id);
                TempData["Success"] = "Đã xóa tin nhắn từ " + message.Name;
            }
            return RedirectToAction("MessageManager");
        }

        // =====================================================
        // CẤU HÌNH NGÂN HÀNG
        // =====================================================
        public ActionResult BankConfig()
        {
            ViewBag.Title = "Cấu hình ngân hàng";
            var service = new BankSettingService();
            var setting = service.Get();
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BankConfig(BankSetting model)
        {
            if (string.IsNullOrWhiteSpace(model.BankName) || string.IsNullOrWhiteSpace(model.AccountNumber))
            {
                ViewBag.Error = "Vui lòng nhập tên ngân hàng và số tài khoản";
                return View(model);
            }

            var service = new BankSettingService();
            service.Update(model);

            TempData["Success"] = "Đã cập nhật cấu hình ngân hàng!";
            return RedirectToAction("BankConfig");
        }

        // =====================================================
        // QUẢN LÝ VOUCHER
        // =====================================================
        public ActionResult CouponManager()
        {
            ViewBag.Title = "Quản lý Voucher";
            return View(CouponData.Coupons);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCoupon(string code, decimal discountAmount, decimal minOrderValue, DateTime? expiryDate)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                TempData["Error"] = "Vui lòng nhập mã.";
                return RedirectToAction("CouponManager");
            }

            if (!expiryDate.HasValue)
            {
                TempData["Error"] = "Vui lòng chọn ngày hết hạn.";
                return RedirectToAction("CouponManager");
            }

            if (CouponData.Coupons.Any(c => c.Code.ToLower() == code.ToLower()))
            {
                TempData["Error"] = "Mã này đã tồn tại. Vui lòng dùng mã khác.";
                return RedirectToAction("CouponManager");
            }

            CouponData.Coupons.Add(new Coupon
            {
                Code = code.ToUpper(),
                DiscountAmount = discountAmount,
                MinOrderValue = minOrderValue,
                ExpiryDate = expiryDate.Value,
                IsActive = true
            });

            TempData["Success"] = "Đã thêm mã: " + code.ToUpper();
            return RedirectToAction("CouponManager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCoupon(string code)
        {
            var coupon = CouponData.Coupons.FirstOrDefault(c => c.Code == code);
            if (coupon != null)
            {
                CouponData.Coupons.Remove(coupon);
                TempData["Success"] = "Đã xóa mã: " + code;
            }
            return RedirectToAction("CouponManager");
        }

        // =====================================================
        // QUẢN LÝ DANH MỤC
        // =====================================================
        public ActionResult CategoryManager()
        {
            ViewBag.Title = "Quản lý danh mục";
            var categories = _productService.GetAllCategories();
            return View(categories);
        }

        public ActionResult CreateCategory()
        {
            ViewBag.Title = "Thêm danh mục";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(Category model)
        {
            if (ModelState.IsValid)
            {
                _productService.AddCategory(model);
                TempData["Success"] = "Đã thêm danh mục: " + model.Name;
                return RedirectToAction("CategoryManager");
            }
            return View(model);
        }

        public ActionResult EditCategory(int id)
        {
            var category = _productService.GetCategoryById(id);
            if (category == null) return HttpNotFound();

            ViewBag.Title = "Sửa danh mục";
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(Category model)
        {
            if (ModelState.IsValid)
            {
                _productService.UpdateCategory(model);
                TempData["Success"] = "Đã cập nhật danh mục: " + model.Name;
                return RedirectToAction("CategoryManager");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategory(int id)
        {
            if (_productService.HasProducts(id))
            {
                TempData["Error"] = "Không thể xóa! Danh mục này đang chứa sản phẩm.";
                return RedirectToAction("CategoryManager");
            }

            _productService.DeleteCategory(id);
            TempData["Success"] = "Đã xóa danh mục thành công.";
            return RedirectToAction("CategoryManager");
        }

        // =====================================================
        // QUẢN LÝ NGƯỜI DÙNG (ADMIN)
        // =====================================================
        public ActionResult UserManager()
        {
            ViewBag.Title = "Quản lý người dùng";
            var users = db.AppUsers.OrderBy(u => u.Id).ToList(); // Lấy danh sách user từ DB
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleUserStatus(int id)
        {
            var user = db.AppUsers.Find(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                db.SaveChanges();
                TempData["Success"] = "Đã cập nhật trạng thái tài khoản: " + user.Email;
            }
            return RedirectToAction("UserManager");
        }
    }
}